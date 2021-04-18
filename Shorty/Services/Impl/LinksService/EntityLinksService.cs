using Microsoft.EntityFrameworkCore;
using Shorty.Models;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Shorty.Services.Impl.LinksService
{
    public class EntityLinksService : ILinksService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILinkIdGeneratorService _generatorService;
        private readonly ILinksNormalizationService _normalization;

        private readonly bool _ensureSingleRecord;

        public EntityLinksService
            (AppDbContext context, 
            ILinkIdGeneratorService generatorService,
            ILinksNormalizationService normalization,
            bool ensureSingleRecord = true)
        {
            _dbContext = context;
            _generatorService = generatorService;
            _normalization = normalization;

            _ensureSingleRecord = ensureSingleRecord;
        }

        public async Task<Link> GetLinkById(string id)
        {
            var link = await _dbContext.Links.FirstOrDefaultAsync(l => l.Id == id);
            if (link == null)
                throw new LinkNotFoundException(id);
            return link;
        }

        public async Task<Link> GetLinkByUrl(string url)
        {
            var uri = _normalization.NormalizeLink(url);
            var hash = GetHash(_normalization.ConvertToString(uri));
            var link = await _dbContext.Links.FirstOrDefaultAsync(l => l.UrlHash == hash);
            if (link == null)
                throw new LinkNotFoundException($"url={url} (hash={hash})");
            return link;
        }

        public async Task<Link> IncrementLink(Link link)
        {
            link.Hits++;
            await _dbContext.SaveChangesAsync();
            return link;
        }

        public async Task<Link> GetOrCreateLink(Uri uri)
        {
            string normalizedUrl = _normalization.ConvertToString(uri);
            string hash = GetHash(normalizedUrl);

            Link link;

            if (_ensureSingleRecord)
            {
                link = await _dbContext.Links.FirstOrDefaultAsync(l => l.UrlHash == hash);
                if (link != null)
                    return link;
            }

            link = new Link
            {
                Id = _generatorService.GenerateId(normalizedUrl),
                CreatedAt = DateTime.UtcNow,
                Url = normalizedUrl,
                UrlHash = hash
            };
            _dbContext.Add(link);
            await _dbContext.SaveChangesAsync();
            return link;
        }

       
        #region static

        private static string GetHash(string s)
        {
            var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(s);
            var hashBytes = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hashBytes);
        }
        
        #endregion
    }
}
