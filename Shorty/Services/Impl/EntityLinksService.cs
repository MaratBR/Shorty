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
        private AppDbContext dbContext;

        public EntityLinksService(AppDbContext context)
        {
            dbContext = context;
        }

        public async Task<string> GenerateLinkId()
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
            while (await dbContext.Links.AnyAsync(link => link.Id == id));

            return id;
        }

        public Task<Link> GetLinkById(string id)
        {
            var link = dbContext.Links.FirstOrDefaultAsync(link => link.Id == id);
            if (link == null)
                throw new 
        }

        public Task<Link> GetLinkByUrl(string url)
        {
            
        }

        public async Task<Link> GetOrCreateLink(string url)
        {
            string normalizedUrl = NormalizeURL(url);
            string hash = GetHash(normalizedUrl);

            var link = await dbContext.Links.FirstOrDefaultAsync(link => link.UrlHash == hash);
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
            dbContext.Add(link);
            await dbContext.SaveChangesAsync();
            return link;
        }


        private const string CHARS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private static readonly Random random = new Random();

        private static string GenerateId(int length)
        {
            char[] chars = new char[length];
            for (int i = 0; i < length; i++)
                chars[i] = CHARS[random.Next(CHARS.Length)];
            return new string(chars);
        }

        private static string _NormalizeURL(string url)
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

        public string NormalizeURL(string url)
        {
            return _NormalizeURL(url);
        }
    }
}
