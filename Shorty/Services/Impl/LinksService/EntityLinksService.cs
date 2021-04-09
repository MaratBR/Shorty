using Microsoft.EntityFrameworkCore;
using Shorty.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Shorty.Utils;

namespace Shorty.Services.Impl
{
    public class EntityLinksService : ILinksService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILinkIdGeneratorService _generatorService;

        public EntityLinksService(AppDbContext context, ILinkIdGeneratorService generatorService)
        {
            _dbContext = context;
            _generatorService = generatorService;
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
            var hash = GetHash(NormalizeUrl(url));
            var link = await _dbContext.Links.FirstOrDefaultAsync(l => l.UrlHash == hash);
            if (link == null)
                throw new LinkNotFoundException($"url={url} (hash={hash})");
            return link;
        }

        public async Task<Link> GetOrCreateLink(string url)
        {
            string normalizedUrl = NormalizeUrl(url);
            string hash = GetHash(normalizedUrl);

            var link = await _dbContext.Links.FirstOrDefaultAsync(l => l.UrlHash == hash);
            if (link != null)
                return link;

            link = new Link
            {
                Id = _generatorService.GenerateId(normalizedUrl),
                CreatedAt = DateTime.UtcNow,
                Hits = 0,
                Url = normalizedUrl,
                UrlHash = hash
            };
            _dbContext.Add(link);
            await _dbContext.SaveChangesAsync();
            return link;
        }

       
        #region static
        
        private static string NormalizeUrl(string url)
        {
            var builder = new UriBuilder(url);
            
            if (builder.Scheme != "https" && builder.Scheme != "http")
                throw new InvalidUrlException($"unexpected schema - {builder.Scheme}, only http/https is supported");

            return builder.ToString();
        }

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
