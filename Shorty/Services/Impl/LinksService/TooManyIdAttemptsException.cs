using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shorty.Services.Impl
{

    [Serializable]
    public class TooManyIdAttemptsException : LinkServiceException
    {
        public TooManyIdAttemptsException(int attempts) 
            : base($"failed to generate random id after {attempts} attempts")
        {
        }
    }
}
