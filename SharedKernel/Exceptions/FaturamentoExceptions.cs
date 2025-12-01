using System;

namespace SharedKernel.Exceptions
{

    
    /// <summary>
    /// Exception específica para serviços de faturamento
    /// </summary>
    public class FaturamentoServiceException : BaseException
    {
        public FaturamentoServiceException(string message) 
            : base(message, "FATURAMENTO_SERVICE_ERROR", 500)
        {
        }

        public FaturamentoServiceException(string message, Exception innerException) 
            : base(message, "FATURAMENTO_SERVICE_ERROR", innerException, 500)
        {
        }
    }

    /// <summary>
    /// Exception para faturamento parcial não encontrado
    /// </summary>
    public class FaturamentoParcialNotFoundException : EntityNotFoundException
    {
        public FaturamentoParcialNotFoundException(object faturamentoId) 
            : base("FaturamentoParcial", faturamentoId)
        {
        }
    }

    /// <summary>
    /// Exception para faturamento diário não encontrado
    /// </summary>
    public class FaturamentoDiarioNotFoundException : EntityNotFoundException
    {
        public FaturamentoDiarioNotFoundException(object faturamentoId) 
            : base("FaturamentoDiario", faturamentoId)
        {
        }
    }

    /// <summary>
    /// Exception para sobreposição de faturamentos
    /// </summary>
    public class FaturamentoOverlapException : BusinessRuleException
    {
        public FaturamentoOverlapException() 
            : base("Existe sobreposição com outro faturamento no mesmo período")
        {
        }
    }

    /// <summary>
    /// Exception para acesso negado à unidade
    /// </summary>
    public class UnidadeAccessDeniedException : AccessDeniedException
    {
        public UnidadeAccessDeniedException(object unidadeId) 
            : base($"Unidade {unidadeId}")
        {
        }
    }

    /// <summary>
    /// Exception para data/hora inválida no faturamento
    /// </summary>
    public class InvalidFaturamentoTimeException : BusinessRuleException
    {
        public InvalidFaturamentoTimeException(string message) 
            : base(message)
        {
        }
    }
}