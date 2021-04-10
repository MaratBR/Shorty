using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shorty.Services.Impl.LinksNormalizationService
{
    public class InvalidUrlException : ServiceException
    {
        public InvalidUrlException(string message) : base($"invalid url: {message}") {}
    }
}
