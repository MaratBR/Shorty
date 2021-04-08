using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shorty.Services.Impl
{
    public class LinkNotFoundException : LinkServiceException
    {
        public LinkNotFoundException(string id){}
    }
}
