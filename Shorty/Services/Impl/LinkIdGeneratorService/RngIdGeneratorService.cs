using System;
using System.Security.Cryptography;
using Base62;
using Shorty.Models;

namespace Shorty.Services.Impl.LinkIdGeneratorService
{
    public class RngIdGeneratorService : ILinkIdGeneratorService
    {
        private readonly RNGCryptoServiceProvider _rngProvider = new RNGCryptoServiceProvider();
        private readonly int _preferredLength;
        private readonly short _bytesRequired;
        
        public RngIdGeneratorService(SharedConfiguration sharedConfiguration)
        {
            _preferredLength = sharedConfiguration.PreferredLinkLength;
            _bytesRequired = Convert.ToInt16(Math.Floor(Math.Log(Math.Pow(62, _preferredLength), 256)));
        }
        
        public string GenerateId(string normalizedUrl)
        {
            byte[] bytes = new byte[_bytesRequired]; 
            _rngProvider.GetBytes(bytes);
            var s = bytes.ToBase62();
            if (s.Length < _preferredLength)
                s = s.PadRight(_preferredLength, '0');
            return s;
        }
    }
}