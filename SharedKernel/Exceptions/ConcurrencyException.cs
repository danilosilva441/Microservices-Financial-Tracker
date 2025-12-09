// SharedKernel/Exceptions/ConcurrencyException.cs
using System;

namespace SharedKernel.Exceptions;

/// <summary>
/// Exceção lançada quando ocorre um conflito de concorrência
/// </summary>
public class ConcurrencyException : BaseException
{
    /// <summary>
    /// Versão esperada da entidade
    /// </summary>
    public object? ExpectedVersion { get; }

    /// <summary>
    /// Versão atual da entidade
    /// </summary>
    public object? CurrentVersion { get; }

    public ConcurrencyException(
        string entityName,
        object? entityId = null,
        object? expectedVersion = null,
        object? currentVersion = null,
        string? message = null,
        Exception? innerException = null)
        : base(
            message ?? GetDefaultMessage(entityName, entityId),
            "CONCURRENCY_CONFLICT",
            ErrorSeverity.Error,
            innerException,
            new 
            { 
                EntityName = entityName, 
                EntityId = entityId,
                ExpectedVersion = expectedVersion,
                CurrentVersion = currentVersion
            })
    {
        ExpectedVersion = expectedVersion;
        CurrentVersion = currentVersion;
    }

    private static string GetDefaultMessage(string entityName, object? entityId)
    {
        return entityId != null
            ? $"Conflito de concorrência ao atualizar {entityName} com ID '{entityId}'. O recurso foi modificado por outro usuário."
            : $"Conflito de concorrência ao atualizar {entityName}. O recurso foi modificado por outro usuário.";
    }
}