using System.Security.Cryptography;
using System.Text;
using Base62;
using Shorty.Models;
using Shorty.Utils;

namespace Shorty.Services.Impl.LinkIdGeneratorService
{
    public class HashIdGeneratorService : ILinkIdGeneratorService
    {
        private readonly int _preferredLength;
        
        public HashIdGeneratorService(SharedConfiguration sharedConfiguration)
        {
            _preferredLength = sharedConfiguration.PreferredLinkLength;
        }
        
        private static readonly SHA256 Sha256 = SHA256.Create();
        
        public string GenerateId(string normalizedUrl)
        {
            var bytes = Encoding.UTF8.GetBytes(normalizedUrl);
            Sha256.ComputeHash(bytes);
            var hashString = Sha256.Hash.ToBase62();
            return hashString.Length > _preferredLength ? hashString.Substring(0, _preferredLength) : hashString;
        }
    }
}