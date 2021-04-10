using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shorty.Services.Impl.LinksService
{
    public class LinkNotFoundException : LinkServiceException
    {
        public LinkNotFoundException(string description) : base($"link not found - {description}") {}
    }
}
