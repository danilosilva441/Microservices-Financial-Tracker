// SharedKernel/Exceptions/DomainException.cs
using System;

namespace SharedKernel.Exceptions;

/// <summary>
/// Exceção lançada quando ocorre uma violação de regra de domínio
/// </summary>
public class DomainException : BaseException
{
    /// <summary>
    /// Nome da entidade onde ocorreu o erro
    /// </summary>
    public string EntityName { get; }

    /// <summary>
    /// ID da entidade (se aplicável)
    /// </summary>
    public object? EntityId { get; }

    public DomainException(
        string entityName,
        string message,
        object? entityId = null,
        Exception? innerException = null,
        object? additionalData = null)
        : base(
            message,
            "DOMAIN_VIOLATION",
            ErrorSeverity.Error,
            innerException,
            additionalData)
    {
        EntityName = entityName;
        EntityId = entityId;
    }
}