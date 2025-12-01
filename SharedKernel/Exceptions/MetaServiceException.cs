using System;

namespace SharedKernel.Exceptions
{
    public class MetaServiceException : BaseException
    {
        public MetaServiceException(string message) 
            : base(message, "META_SERVICE_ERROR", 500)
        {
        }

        public MetaServiceException(string message, Exception innerException) 
            : base(message, "META_SERVICE_ERROR", innerException, 500)
        {
        }
    }
}