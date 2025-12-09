using System;

namespace SharedKernel.Exceptions
{
    public class MetaServiceException : InfrastructureException
    {
        public MetaServiceException(string message) 
            : base(
                "MetaService", 
                "GeneralOperation", 
                ErrorCodes.OperationFailed, 
                message)
        {
        }

        public MetaServiceException(string message, Exception innerException) 
            : base(
                "MetaService", 
                "GeneralOperation", 
                ErrorCodes.OperationFailed, 
                message, 
                innerException)
        {
        }

        public MetaServiceException(string operation, string message) 
            : base(
                "MetaService", 
                operation, 
                ErrorCodes.OperationFailed, 
                message)
        {
        }

        public MetaServiceException(string operation, string message, Exception innerException) 
            : base(
                "MetaService", 
                operation, 
                ErrorCodes.OperationFailed, 
                message, 
                innerException)
        {
        }
    }
}