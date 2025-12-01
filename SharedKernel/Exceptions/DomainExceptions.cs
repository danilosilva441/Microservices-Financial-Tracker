namespace SharedKernel.Exceptions
{
    /// <summary>
    /// Exception para quando uma entidade não é encontrada
    /// </summary>
    public class EntityNotFoundException : BaseException
    {
        public string EntityType { get; }
        public object EntityId { get; }

        public EntityNotFoundException(string entityType, object entityId)
            : base($"{entityType} com ID {entityId} não encontrado", "ENTITY_NOT_FOUND", 404)
        {
            EntityType = entityType;
            EntityId = entityId;
        }
    }

    /// <summary>
    /// Exception para quando o usuário não tem permissão
    /// </summary>
    public class AccessDeniedException : BaseException
    {
        public AccessDeniedException(string resource)
            : base($"Acesso negado ao recurso: {resource}", "ACCESS_DENIED", 403)
        {
        }
    }

    /// <summary>
    /// Exception para validações de negócio
    /// </summary>
    public class BusinessRuleException : BaseException
    {
        public BusinessRuleException(string message)
            : base(message, "BUSINESS_RULE_VIOLATION", 422)
        {
        }

        public BusinessRuleException(string message, Exception innerException)
            : base(message, "BUSINESS_RULE_VIOLATION", innerException, 422)
        {
        }
    }

    /// <summary>
    /// Exception para entidades duplicadas
    /// </summary>
    public class DuplicateEntityException : BaseException
    {
        public DuplicateEntityException(string entityType)
            : base($"{entityType} já existe", "DUPLICATE_ENTITY", 409)
        {
        }
    }

    /// <summary>
    /// Exception para operações inválidas no estado atual
    /// </summary>
    public class InvalidOperationException : BaseException
    {
        public InvalidOperationException(string message)
            : base(message, "INVALID_OPERATION", 422)
        {
        }
    }
    /// <summary>
    /// Exception para operações inválidas no estado atual (DOMÍNIO)
    /// </summary>
    public class InvalidDomainOperationException : BaseException  // ✅ MUDANÇA DE NOME
    {
        public InvalidDomainOperationException(string message)   // ✅ MUDANÇA DE NOME
            : base(message, "INVALID_DOMAIN_OPERATION", 422)    // ✅ MUDANÇA DE NOME
        {
        }
    }
}