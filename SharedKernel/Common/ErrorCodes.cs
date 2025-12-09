namespace SharedKernel
{
    /// <summary>
    /// Centraliza todos os códigos de erro da aplicação (v2.0)
    /// para padronização e fácil referência nos controladores e clientes.
    /// </summary>
    public static class ErrorCodes
    {
        // Códigos de Autenticação (1xxx)
        public const string InvalidCredentials = "AUTH_1001";
        public const string EmailInUse = "AUTH_1002";
        public const string UserNotFound = "AUTH_1003";
        public const string RoleNotFound = "AUTH_1004";
        public const string TenantProvisionFailed = "AUTH_1005";
        public const string AccountLocked = "AUTH_1006";
        public const string PasswordExpired = "AUTH_1007";
        public const string InvalidToken = "AUTH_1008";
        
        // Códigos de Autorização/Hierarquia (2xxx)
        public const string InvalidRoleCreation = "AUTH_2001";
        public const string ManagerTenantRequired = "AUTH_2002";
        public const string InsufficientPermissions = "AUTH_2003";
        public const string CannotModifyOwnRole = "AUTH_2004";
        public const string CannotDeleteOwnAccount = "AUTH_2005";
        public const string UserNoPermission = "AUTH_2006";
        public const string AcessoNegado = "AUTH_2007";
        
        // Códigos de Negócio - BillingService (3xxx)
        public const string UnidadeNotFound = "BILL_3001";
        public const string CategoriaNotFound = "BILL_3002";
        public const string DespesaNotFound = "BILL_3003";
        public const string FechamentoNotFound = "BILL_3004";
        public const string FechamentoJaExiste = "BILL_3005";
        public const string DespesaExceedsBudget = "BILL_3006";
        public const string InvalidDateRange = "BILL_3007";
        public const string DataFuturaNaoPermitida = "BILL_3008";
        public const string NoAlteredStatus = "BILL_3009";
        public const string FaturamentoNotFound = "BILL_3010";
        public const string FaturamentoParcialNotFound = "BILL_3011";
        public const string FaturamentoJaExiste = "BILL_3012";
        public const string OverlappingFaturamento = "BILL_3013";
        
        // Códigos de Validação (4xxx)
        public const string RequiredField = "VALID_4001";
        public const string InvalidEmailFormat = "VALID_4002";
        public const string StringLengthExceeded = "VALID_4003";
        public const string InvalidNumberRange = "VALID_4004";
        public const string InvalidDateFormat = "VALID_4005";
        public const string InvalidFileType = "VALID_4006";
        public const string FileRequired = "VALID_4007";
        
        // Códigos de Sistema/Infraestrutura (5xxx)
        public const string DatabaseConnectionFailed = "SYS_5001";
        public const string ExternalServiceUnavailable = "SYS_5002";
        public const string FileStorageError = "SYS_5003";
        public const string EmailServiceError = "SYS_5004";
        public const string ConcurrentModification = "SYS_5005";
        
        // Códigos Genéricos (9xxx)
        public const string GenericNotFound = "GEN_9001";
        public const string OperationFailed = "GEN_9002";
        public const string InvalidOperation = "GEN_9003";

    
    }

    /// <summary>
    /// Extensões para trabalhar com códigos de erro
    /// </summary>
    public static class ErrorCodesExtensions
    {
        /// <summary>
        /// Obtém a categoria do erro baseado no código
        /// </summary>
        public static string GetErrorCategory(this string errorCode)
        {
            if (string.IsNullOrEmpty(errorCode) || errorCode.Length < 4)
                return "UNKNOWN";

            var prefix = errorCode.Split('_')[0];
            return prefix switch
            {
                "AUTH" => "Authentication",
                "BILL" => "Billing",
                "VALID" => "Validation", 
                "SYS" => "System",
                "GEN" => "Generic",
                _ => "Unknown"
            };
        }

        /// <summary>
        /// Obtém o HTTP Status Code sugerido para o erro
        /// </summary>
        public static int GetSuggestedHttpStatusCode(this string errorCode)
        {
            if (string.IsNullOrEmpty(errorCode))
                return 500;

            var prefix = errorCode.Split('_')[0];
            
            // Para códigos específicos, use switch por código completo
            return errorCode switch
            {
                // Authentication - Unauthorized (401)
                ErrorCodes.InvalidCredentials => 401,
                ErrorCodes.InvalidToken => 401,
                
                // Authentication - Forbidden (403)
                ErrorCodes.AccountLocked => 423, // Locked
                ErrorCodes.PasswordExpired => 403,
                ErrorCodes.InsufficientPermissions => 403,
                ErrorCodes.CannotModifyOwnRole => 403,
                ErrorCodes.CannotDeleteOwnAccount => 403,
                ErrorCodes.UserNoPermission => 403,
                ErrorCodes.AcessoNegado => 403,
                ErrorCodes.InvalidRoleCreation => 403,
                ErrorCodes.ManagerTenantRequired => 403,
                
                // Billing - Not Found (404)
                ErrorCodes.UnidadeNotFound => 404,
                ErrorCodes.CategoriaNotFound => 404,
                ErrorCodes.DespesaNotFound => 404,
                ErrorCodes.FechamentoNotFound => 404,
                ErrorCodes.FaturamentoNotFound => 404,
                ErrorCodes.FaturamentoParcialNotFound => 404,
                
                // Billing - Conflict (409)
                ErrorCodes.FechamentoJaExiste => 409,
                ErrorCodes.FaturamentoJaExiste => 409,
                ErrorCodes.OverlappingFaturamento => 409,
                ErrorCodes.ConcurrentModification => 409,
                
                // Billing - Unprocessable Entity (422)
                ErrorCodes.DespesaExceedsBudget => 422,
                ErrorCodes.InvalidDateRange => 422,
                ErrorCodes.DataFuturaNaoPermitida => 422,
                ErrorCodes.NoAlteredStatus => 422,
                
                // Validation - Bad Request (400)
                ErrorCodes.RequiredField => 400,
                ErrorCodes.InvalidEmailFormat => 400,
                ErrorCodes.StringLengthExceeded => 400,
                ErrorCodes.InvalidNumberRange => 400,
                ErrorCodes.InvalidDateFormat => 400,
                ErrorCodes.InvalidFileType => 400,
                ErrorCodes.FileRequired => 400,
                
                // Generic
                ErrorCodes.GenericNotFound => 404,
                ErrorCodes.InvalidOperation => 409,
                
                // System errors - Internal Server Error (500)
                ErrorCodes.DatabaseConnectionFailed => 500,
                ErrorCodes.ExternalServiceUnavailable => 503,
                ErrorCodes.FileStorageError => 500,
                ErrorCodes.EmailServiceError => 500,
                ErrorCodes.TenantProvisionFailed => 500,
                ErrorCodes.OperationFailed => 500,
                
                // Fallback por prefixo
                "AUTH" => 401,
                "VALID" => 400,
                "BILL" => 422,
                "SYS" => 500,
                "GEN" => 400,
                
                _ => 500
            };
        }

        /// <summary>
        /// Verifica se o erro é do cliente (4xx) ou servidor (5xx)
        /// </summary>
        public static bool IsClientError(this string errorCode)
        {
            var statusCode = errorCode.GetSuggestedHttpStatusCode();
            return statusCode >= 400 && statusCode < 500;
        }

        /// <summary>
        /// Verifica se o erro é recuperável
        /// </summary>
        public static bool IsRecoverable(this string errorCode)
        {
            var nonRecoverableCodes = new[]
            {
                ErrorCodes.InvalidCredentials,
                ErrorCodes.EmailInUse,
                ErrorCodes.UserNotFound,
                ErrorCodes.RoleNotFound,
                ErrorCodes.AccountLocked,
                ErrorCodes.InvalidToken,
                ErrorCodes.InsufficientPermissions,
                ErrorCodes.AcessoNegado,
                ErrorCodes.UnidadeNotFound,
                ErrorCodes.CategoriaNotFound,
                ErrorCodes.DespesaNotFound,
                ErrorCodes.FechamentoNotFound,
                ErrorCodes.FaturamentoNotFound,
                ErrorCodes.FaturamentoParcialNotFound
            };

            return !nonRecoverableCodes.Contains(errorCode);
        }
    }
}