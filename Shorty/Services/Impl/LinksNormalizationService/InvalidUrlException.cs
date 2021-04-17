using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shorty.Services.Impl.LinksNormalizationService
{
    public class InvalidUrlException : ServiceException
    {
        public InvalidUrlException(string message) : base($"invalid url: {message}") {}
        
        public InvalidUrlException(Exception inner) 
            : base("invalid url, see base exception for more detail", inner) {}
    }
}
