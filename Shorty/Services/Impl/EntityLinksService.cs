using Microsoft.EntityFrameworkCore;
using Shorty.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Shorty.Services.Impl
{
    public class EntityLinksService : ILinksService
    {
        private readonly AppDbContext _dbContext;

        public EntityLinksService(AppDbContext context)
        {
            _dbContext = context;
        }

        private async Task<string> GenerateLinkId()
        {
            string id;
            int length = 4;
            int attempt = 1;
            do
            {
                if (attempt > 20)
                    throw new TooManyIdAttemptsException(attempt);

                id = GenerateId(length);

                if (attempt % 4 == 0)
                    length++;

               attempt++;
            }
            while (await _dbContext.Links.AnyAsync(link => link.Id == id));

            return id;
        }

        public async Task<Link> GetLinkById(string id)
        {
            var link = await _dbContext.Links.FirstOrDefaultAsync(link => link.Id == id);
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

            string id = await GenerateLinkId();
            link = new Link
            {
                Id = id,
                CreatedAt = DateTime.UtcNow,
                Hits = 0,
                Url = normalizedUrl,
                UrlHash = hash
            };
            _dbContext.Add(link);
            await _dbContext.SaveChangesAsync();
            return link;
        }


        private const string Chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private static readonly Random RandomInstance = new Random();

        private static string GenerateId(int length)
        {
            char[] chars = new char[length];
            for (int i = 0; i < length; i++)
                chars[i] = Chars[RandomInstance.Next(Chars.Length)];
            return new string(chars);
        }

        private static string NormalizeUrl(string url)
        {
            Uri uri = new Uri(url);

            if (uri.Scheme != "https" && uri.Scheme != "http")
                throw new InvalidUrlException($"unexpected schema - {uri.Scheme}, only http/https is supported");
            return uri.ToString();
        }

        private static string GetHash(string s)
        {
            var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(s);
            var hashBytes = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hashBytes);
        }
    }
}
