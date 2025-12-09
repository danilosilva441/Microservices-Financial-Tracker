// SharedKernel/Exceptions/InfrastructureException.cs
using System;

namespace SharedKernel.Exceptions;

/// <summary>
/// Exceção lançada quando ocorrem erros de infraestrutura
/// </summary>
public class InfrastructureException : BaseException
{
    /// <summary>
    /// Componente de infraestrutura que falhou
    /// </summary>
    public string Component { get; }

    /// <summary>
    /// Operação que estava sendo executada
    /// </summary>
    public string Operation { get; }

    public InfrastructureException(
        string component,
        string operation,
        string errorCode = ErrorCodes.OperationFailed,
        string? message = null,
        Exception? innerException = null,
        object? additionalData = null)
        : base(
            errorCode,
            message ?? GetDefaultMessage(component, operation, errorCode),
            ErrorSeverity.Error,
            innerException,
            additionalData)
    {
        Component = component;
        Operation = operation;
    }

    private static string GetDefaultMessage(
        string component, 
        string operation, 
        string errorCode)
    {
        var defaultMessage = errorCode.GetDefaultMessage();
        if (defaultMessage != ErrorMessages.OperationFailed)
        {
            return defaultMessage;
        }

        return $"Erro no componente '{component}' durante a operação: {operation}";
    }

    /// <summary>
    /// Cria uma InfrastructureException para erro de conexão com banco de dados
    /// </summary>
    public static InfrastructureException DatabaseError(
        string operation,
        Exception innerException,
        object? additionalData = null)
    {
        return new InfrastructureException(
            component: "Database",
            operation: operation,
            errorCode: ErrorCodes.DatabaseConnectionFailed,
            message: ErrorMessages.DatabaseConnectionFailed,
            innerException: innerException,
            additionalData: additionalData);
    }

    /// <summary>
    /// Cria uma InfrastructureException para serviço externo indisponível
    /// </summary>
    public static InfrastructureException ExternalServiceUnavailable(
        string serviceName,
        string operation,
        Exception innerException,
        object? additionalData = null)
    {
        return new InfrastructureException(
            component: "ExternalService",
            operation: operation,
            errorCode: ErrorCodes.ExternalServiceUnavailable,
            message: ErrorMessages.ExternalServiceUnavailable,
            innerException: innerException,
            additionalData: additionalData);
    }
}