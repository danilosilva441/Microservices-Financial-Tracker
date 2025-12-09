// SharedKernel/Exceptions/NotFoundException.cs
using System;

namespace SharedKernel.Exceptions;

/// <summary>
/// Exceção lançada quando um recurso não é encontrado (genérico)
/// </summary>
public class NotFoundException : BaseException
{
    /// <summary>
    /// Tipo do recurso não encontrado
    /// </summary>
    public string ResourceType { get; }

    /// <summary>
    /// Identificador do recurso não encontrado
    /// </summary>
    public object? ResourceId { get; }

    /// <summary>
    /// Inicializa uma nova instância da classe NotFoundException
    /// </summary>
    /// <param name="resourceType">Tipo do recurso não encontrado</param>
    /// <param name="resourceId">Identificador do recurso (opcional)</param>
    /// <param name="errorCode">Código de erro (padrão: GenericNotFound)</param>
    /// <param name="message">Mensagem de erro personalizada (opcional)</param>
    /// <param name="innerException">Exceção interna (opcional)</param>
    public NotFoundException(
        string resourceType,
        object? resourceId = null,
        string errorCode = ErrorCodes.GenericNotFound,
        string? message = null,
        Exception? innerException = null)
        : base(
            errorCode,
            message ?? GetDefaultMessage(resourceType, resourceId, errorCode),
            ErrorSeverity.Warning,
            innerException,
            new { ResourceType = resourceType, ResourceId = resourceId })
    {
        ResourceType = resourceType;
        ResourceId = resourceId;
    }

    /// <summary>
    /// Inicializa uma nova instância da classe NotFoundException
    /// </summary>
    /// <param name="errorCode">Código de erro específico</param>
    /// <param name="message">Mensagem de erro</param>
    /// <param name="innerException">Exceção interna (opcional)</param>
    public NotFoundException(
        string errorCode,
        string message,
        Exception? innerException = null)
        : base(errorCode, message, ErrorSeverity.Warning, innerException)
    {
        ResourceType = "Unknown";
    }

    private static string GetDefaultMessage(
        string resourceType, 
        object? resourceId, 
        string errorCode)
    {
        // Se já temos uma mensagem específica para este código de erro, use-a
        var defaultMessage = errorCode.GetDefaultMessage();
        if (defaultMessage != ErrorMessages.OperationFailed)
        {
            return defaultMessage;
        }

        // Caso contrário, gere uma mensagem genérica
        return resourceId != null
            ? $"{resourceType} com ID '{resourceId}' não foi encontrado."
            : $"{resourceType} não foi encontrado.";
    }
}

// Extensão para obter mensagens padrão
public static class ErrorCodeExtensions
{
    public static string GetDefaultMessage(this string errorCode)
    {
        return errorCode switch
        {
            ErrorCodes.InvalidCredentials => ErrorMessages.InvalidCredentials,
            ErrorCodes.EmailInUse => ErrorMessages.EmailInUse,
            ErrorCodes.UserNotFound => ErrorMessages.UserNotFound,
            // ... mapeie todas as mensagens aqui
            _ => ErrorMessages.OperationFailed
        };
    }
}