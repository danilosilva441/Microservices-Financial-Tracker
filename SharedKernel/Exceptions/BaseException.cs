// SharedKernel/Exceptions/BaseException.cs
using System;

namespace SharedKernel.Exceptions;

/// <summary>
/// Classe base para todas as exceções personalizadas da aplicação
/// </summary>
public abstract class BaseException : Exception
{
    /// <summary>
    /// Código de erro personalizado
    /// </summary>
    public string ErrorCode { get; }

    /// <summary>
    /// Severidade do erro
    /// </summary>
    public ErrorSeverity Severity { get; }

    /// <summary>
    /// TimeStamp do erro
    /// </summary>
    public DateTime TimeStamp { get; }

    /// <summary>
    /// Dados adicionais sobre o erro
    /// </summary>
    public object? AdditionalData { get; }

    /// <summary>
    /// HTTP Status Code sugerido
    /// </summary>
    public int SuggestedHttpStatusCode => ErrorCode.GetSuggestedHttpStatusCode();

    protected BaseException(
        string errorCode,
        string message,
        ErrorSeverity severity = ErrorSeverity.Error,
        Exception? innerException = null,
        object? additionalData = null)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
        Severity = severity;
        TimeStamp = DateTime.UtcNow;
        AdditionalData = additionalData;
    }

    protected BaseException(
        string errorCode,
        ErrorSeverity severity = ErrorSeverity.Error,
        Exception? innerException = null,
        object? additionalData = null)
        : this(errorCode, GetDefaultMessage(errorCode), severity, innerException, additionalData)
    {
    }

    private static string GetDefaultMessage(string errorCode)
    {
        return errorCode switch
        {
            // Authentication
            ErrorCodes.InvalidCredentials => ErrorMessages.InvalidCredentials,
            ErrorCodes.EmailInUse => ErrorMessages.EmailInUse,
            ErrorCodes.UserNotFound => ErrorMessages.UserNotFound,
            ErrorCodes.RoleNotFound => ErrorMessages.RoleNotFound,
            ErrorCodes.TenantProvisionFailed => ErrorMessages.TenantProvisionFailed,
            ErrorCodes.AccountLocked => ErrorMessages.AccountLocked,
            ErrorCodes.PasswordExpired => ErrorMessages.PasswordExpired,
            ErrorCodes.InvalidToken => ErrorMessages.InvalidToken,

            // Authorization
            ErrorCodes.InvalidRoleCreation => ErrorMessages.InvalidRoleCreation,
            ErrorCodes.ManagerTenantRequired => ErrorMessages.ManagerTenantRequired,
            ErrorCodes.InsufficientPermissions => ErrorMessages.InsufficientPermissions,
            ErrorCodes.CannotModifyOwnRole => ErrorMessages.CannotModifyOwnRole,
            ErrorCodes.CannotDeleteOwnAccount => ErrorMessages.CannotDeleteOwnAccount,
            ErrorCodes.UserNoPermission => ErrorMessages.UserNoPermission,
            ErrorCodes.AcessoNegado => ErrorMessages.AcessoNegado,

            // Billing
            ErrorCodes.UnidadeNotFound => ErrorMessages.UnidadeNotFound,
            ErrorCodes.CategoriaNotFound => ErrorMessages.CategoriaNotFound,
            ErrorCodes.DespesaNotFound => ErrorMessages.DespesaNotFound,
            ErrorCodes.FechamentoNotFound => ErrorMessages.FechamentoNotFound,
            ErrorCodes.FechamentoJaExiste => ErrorMessages.FechamentoJaExiste,
            ErrorCodes.DespesaExceedsBudget => ErrorMessages.DespesaExceedsBudget,
            ErrorCodes.InvalidDateRange => ErrorMessages.InvalidDateRange,
            ErrorCodes.DataFuturaNaoPermitida => ErrorMessages.DataFuturaNaoPermitida,
            ErrorCodes.NoAlteredStatus => ErrorMessages.NoAlteredStatus,
            ErrorCodes.FaturamentoNotFound => ErrorMessages.FaturamentoNotFound,
            ErrorCodes.FaturamentoParcialNotFound => ErrorMessages.FaturamentoParcialNotFound,
            ErrorCodes.FaturamentoJaExiste => ErrorMessages.FaturamentoJaExiste,
            ErrorCodes.OverlappingFaturamento => ErrorMessages.OverlappingFaturamento,

            // Validation
            ErrorCodes.RequiredField => ErrorMessages.RequiredField,
            ErrorCodes.InvalidEmailFormat => ErrorMessages.InvalidEmailFormat,
            ErrorCodes.StringLengthExceeded => ErrorMessages.StringLengthExceeded,
            ErrorCodes.InvalidNumberRange => ErrorMessages.InvalidNumberRange,
            ErrorCodes.InvalidDateFormat => ErrorMessages.InvalidDateFormat,
            ErrorCodes.InvalidFileType => ErrorMessages.InvalidFileType,
            ErrorCodes.FileRequired => ErrorMessages.FileRequired,

            // System
            ErrorCodes.DatabaseConnectionFailed => ErrorMessages.DatabaseConnectionFailed,
            ErrorCodes.ExternalServiceUnavailable => ErrorMessages.ExternalServiceUnavailable,
            ErrorCodes.FileStorageError => ErrorMessages.FileStorageError,
            ErrorCodes.EmailServiceError => ErrorMessages.EmailServiceError,
            ErrorCodes.ConcurrentModification => ErrorMessages.ConcurrentModification,

            // Generic
            ErrorCodes.GenericNotFound => ErrorMessages.GenericNotFound,
            ErrorCodes.OperationFailed => ErrorMessages.OperationFailed,
            ErrorCodes.InvalidOperation => ErrorMessages.InvalidOperation,

            _ => ErrorMessages.OperationFailed
        };
    }

    public enum ErrorSeverity
    {
        Info,
        Warning,
        Error,
        Critical
    }
}