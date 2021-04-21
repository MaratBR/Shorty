using Microsoft.EntityFrameworkCore;
using Shorty.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SQLitePCL;

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

        private static readonly Regex CustomIdRegex = new Regex(@"^[a-zA-Z0-9-_@$]{2,}$");

        public async Task<Link> CreateLink(Uri uri, string customId)
        {
            if (!CustomIdRegex.IsMatch(customId))
                throw new InvalidIdException(customId, "does not match the pattern [a-zA-Z0-9]{2,}");
            string normalizedUrl = _normalization.ConvertToString(uri);
            string hash = GetHash(normalizedUrl);

            if (await _dbContext.Links.Where(l => l.Id == customId).CountAsync() > 0)
                throw new LinkAlreadyExistsException(customId);
            
            var link = new Link
            {
                Id = customId,
                CreatedAt = DateTime.UtcNow,
                Url = normalizedUrl,
                UrlHash = hash
            };
            _dbContext.Add(link);
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

        public async Task DeleteLink(string linkId)
        {
            var link = await _dbContext.Links.FirstOrDefaultAsync(l => l.Id == linkId);
            if (link != null)
            {
                _dbContext.Links.Remove(link);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<ILinksService.LinksStats> GetStats()
        {
            var result = await Task.WhenAll(
                _dbContext.Links.LongCountAsync(),
                _dbContext.Links.Where(l => l.CreatedAt >= DateTime.UtcNow.Date).LongCountAsync()
                );
            return new ILinksService.LinksStats
            {
                TotalCount = result[0],
                CountToday = result[1]
            };
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
