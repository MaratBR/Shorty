using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shorty.Services.Impl
{

    [Serializable]
    public class LinkServiceException : ServiceException
    {
        public LinkServiceException() { }
        public LinkServiceException(string message) : base(message) { }
        public LinkServiceException(string message, Exception inner) : base(message, inner) { }
        protected LinkServiceException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
