using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shorty.Services.Impl
{
    public class InvalidUrlException : LinkServiceException
    {
        public InvalidUrlException(string message) : base($"invalid url: {message}") {}
    }
}
