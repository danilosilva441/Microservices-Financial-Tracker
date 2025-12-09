// SharedKernel/Exceptions/BusinessException.cs
using System;

namespace SharedKernel.Exceptions;

/// <summary>
/// Exceção lançada quando uma regra de negócio é violada (genérico)
/// </summary>
public class BusinessException : BaseException
{
    /// <summary>
    /// Código da regra de negócio violada
    /// </summary>
    public string BusinessRuleCode { get; }

    /// <summary>
    /// Inicializa uma nova instância da classe BusinessException
    /// </summary>
    /// <param name="businessRuleCode">Código da regra de negócio</param>
    /// <param name="errorCode">Código de erro associado</param>
    /// <param name="message">Mensagem de erro</param>
    /// <param name="innerException">Exceção interna (opcional)</param>
    /// <param name="additionalData">Dados adicionais (opcional)</param>
    public BusinessException(
        string businessRuleCode,
        string errorCode,
        string message,
        Exception? innerException = null,
        object? additionalData = null)
        : base(
            errorCode,
            message,
            ErrorSeverity.Error,
            innerException,
            additionalData)
    {
        BusinessRuleCode = businessRuleCode;
    }

    /// <summary>
    /// Cria uma BusinessException com código de erro e mensagem do SharedKernel
    /// </summary>
    public static BusinessException Create(
        string errorCode,
        object? additionalData = null)
    {
        var message = errorCode.GetDefaultMessage();
        return new BusinessException(
            errorCode,
            errorCode,
            message,
            additionalData: additionalData);
    }

    /// <summary>
    /// Cria uma BusinessException para fechamento já existente
    /// </summary>
    public static BusinessException FechamentoJaExiste(DateTime data)
    {
        return new BusinessException(
            "FECHAMENTO_JA_EXISTE",
            ErrorCodes.FechamentoJaExiste,
            ErrorMessages.FechamentoJaExiste,
            additionalData: new { Data = data });
    }

    /// <summary>
    /// Cria uma BusinessException para despesa excede orçamento
    /// </summary>
    public static BusinessException DespesaExceedsBudget(
        decimal valorDespesa, 
        decimal orcamentoRestante)
    {
        return new BusinessException(
            "DESPESA_EXCEDE_ORCAMENTO",
            ErrorCodes.DespesaExceedsBudget,
            ErrorMessages.DespesaExceedsBudget,
            additionalData: new 
            { 
                ValorDespesa = valorDespesa, 
                OrcamentoRestante = orcamentoRestante 
            });
    }

    /// <summary>
    /// Cria uma BusinessException para acesso negado
    /// </summary>
    public static BusinessException AcessoNegado(string? recurso = null)
    {
        var message = string.IsNullOrEmpty(recurso) 
            ? ErrorMessages.AcessoNegado 
            : $"{ErrorMessages.AcessoNegado} Recurso: {recurso}";
        
        return new BusinessException(
            "ACESSO_NEGADO",
            ErrorCodes.AcessoNegado,
            message);
    }
}