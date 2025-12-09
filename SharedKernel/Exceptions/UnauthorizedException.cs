// SharedKernel/Exceptions/UnauthorizedException.cs
using System;

namespace SharedKernel.Exceptions;

/// <summary>
/// Exceção lançada quando um usuário não tem autorização para acessar um recurso
/// </summary>
public class UnauthorizedException : BaseException
{
    /// <summary>
    /// ID do usuário que tentou acessar (opcional)
    /// </summary>
    public string? UserId { get; }

    /// <summary>
    /// Recurso que o usuário tentou acessar (opcional)
    /// </summary>
    public string? Resource { get; }

    /// <summary>
    /// Ação que o usuário tentou realizar (opcional)
    /// </summary>
    public string? Action { get; }

    public UnauthorizedException(
        string? userId = null,
        string? resource = null,
        string? action = null,
        string errorCode = ErrorCodes.AcessoNegado,
        string? message = null,
        Exception? innerException = null)
        : base(
            errorCode,
            message ?? GetDefaultMessage(errorCode, userId, resource, action),
            ErrorSeverity.Warning,
            innerException,
            new { UserId = userId, Resource = resource, Action = action })
    {
        UserId = userId;
        Resource = resource;
        Action = action;
    }

    private static string GetDefaultMessage(
        string errorCode, 
        string? userId, 
        string? resource, 
        string? action)
    {
        // Use a mensagem padrão do código de erro
        var defaultMessage = errorCode.GetDefaultMessage();
        
        if (!string.IsNullOrEmpty(defaultMessage) && 
            defaultMessage != ErrorMessages.OperationFailed)
        {
            return defaultMessage;
        }

        // Caso contrário, gere uma mensagem genérica
        var userInfo = !string.IsNullOrEmpty(userId) ? $" (User ID: {userId})" : "";
        var resourceInfo = !string.IsNullOrEmpty(resource) ? $" o recurso '{resource}'" : " este recurso";
        var actionInfo = !string.IsNullOrEmpty(action) ? $" para {action}" : "";

        return $"Acesso não autorizado{userInfo} para acessar{resourceInfo}{actionInfo}.";
    }

    /// <summary>
    /// Cria uma exceção para credenciais inválidas
    /// </summary>
    public static UnauthorizedException InvalidCredentials()
    {
        return new UnauthorizedException(
            errorCode: ErrorCodes.InvalidCredentials,
            message: ErrorMessages.InvalidCredentials);
    }

    /// <summary>
    /// Cria uma exceção para token inválido
    /// </summary>
    public static UnauthorizedException InvalidToken()
    {
        return new UnauthorizedException(
            errorCode: ErrorCodes.InvalidToken,
            message: ErrorMessages.InvalidToken);
    }

    /// <summary>
    /// Cria uma exceção para permissões insuficientes
    /// </summary>
    public static UnauthorizedException InsufficientPermissions(
        string? userId = null, 
        string? resource = null, 
        string? action = null)
    {
        return new UnauthorizedException(
            userId: userId,
            resource: resource,
            action: action,
            errorCode: ErrorCodes.InsufficientPermissions,
            message: ErrorMessages.InsufficientPermissions);
    }
}