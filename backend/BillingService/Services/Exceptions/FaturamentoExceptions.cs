using System;
using SharedKernel;
using SharedKernel.Exceptions;

namespace BillingService.Services.Exceptions  // Mudei o namespace!
{
    /// <summary>
    /// Exception específica para serviços de faturamento
    /// </summary>
    public class FaturamentoServiceException : InfrastructureException
    {
        public FaturamentoServiceException(string message) 
            : base(
                "FaturamentoService", 
                "GeneralOperation", 
                ErrorCodes.OperationFailed, 
                message)
        {
        }

        public FaturamentoServiceException(string message, Exception innerException) 
            : base(
                "FaturamentoService", 
                "GeneralOperation", 
                ErrorCodes.OperationFailed, 
                message, 
                innerException)
        {
        }
    }

    /// <summary>
    /// Exception para faturamento não encontrado
    /// </summary>
    public class FaturamentoNotFoundException : NotFoundException
    {
        public FaturamentoNotFoundException(Guid faturamentoId) 
            : base(
                "Faturamento", 
                faturamentoId, 
                ErrorCodes.FaturamentoNotFound,
                $"Faturamento com ID '{faturamentoId}' não encontrado.")
        {
        }
    }

    /// <summary>
    /// Exception para faturamento parcial não encontrado
    /// </summary>
    public class FaturamentoParcialNotFoundException : NotFoundException
    {
        public FaturamentoParcialNotFoundException(Guid faturamentoParcialId) 
            : base(
                "FaturamentoParcial", 
                faturamentoParcialId, 
                ErrorCodes.FaturamentoParcialNotFound,
                $"Faturamento parcial com ID '{faturamentoParcialId}' não encontrado.")
        {
        }
    }

    /// <summary>
    /// Exception para faturamento diário não encontrado
    /// </summary>
    public class FaturamentoDiarioNotFoundException : NotFoundException
    {
        public FaturamentoDiarioNotFoundException(Guid faturamentoDiarioId) 
            : base(
                "FaturamentoDiario", 
                faturamentoDiarioId, 
                ErrorCodes.FaturamentoNotFound,
                $"Faturamento diário com ID '{faturamentoDiarioId}' não encontrado.")
        {
        }
    }

    /// <summary>
    /// Exception para sobreposição de faturamentos
    /// </summary>
    public class FaturamentoOverlapException : BusinessException
    {
        public FaturamentoOverlapException() 
            : base(
                "OVERLAPPING_FATURAMENTO", 
                ErrorCodes.OverlappingFaturamento,
                ErrorMessages.OverlappingFaturamento)
        {
        }

        public FaturamentoOverlapException(DateTime inicio1, DateTime fim1, DateTime inicio2, DateTime fim2) 
            : base(
                "OVERLAPPING_FATURAMENTO", 
                ErrorCodes.OverlappingFaturamento,
                $"Existe sobreposição com outro faturamento: {inicio1:HH:mm}-{fim1:HH:mm} com {inicio2:HH:mm}-{fim2:HH:mm}",
                additionalData: new 
                { 
                    Periodo1 = new { Inicio = inicio1, Fim = fim1 },
                    Periodo2 = new { Inicio = inicio2, Fim = fim2 }
                })
        {
        }
    }

    /// <summary>
    /// Exception para acesso negado à unidade
    /// </summary>
    public class UnidadeAccessDeniedException : UnauthorizedException
    {
        public UnidadeAccessDeniedException(Guid unidadeId) 
            : base(
                resource: $"Unidade/{unidadeId}",
                errorCode: ErrorCodes.AcessoNegado,
                message: $"Acesso negado à unidade '{unidadeId}'.")
        {
        }
    }

    /// <summary>
    /// Exception para data/hora inválida no faturamento
    /// </summary>
    public class InvalidFaturamentoTimeException : BusinessException
    {
        public InvalidFaturamentoTimeException(string message) 
            : base(
                "INVALID_FATURAMENTO_TIME", 
                ErrorCodes.DataFuturaNaoPermitida,
                message)
        {
        }

        public InvalidFaturamentoTimeException(string message, DateTime dataHora) 
            : base(
                "INVALID_FATURAMENTO_TIME", 
                ErrorCodes.DataFuturaNaoPermitida,
                message,
                additionalData: new { DataHora = dataHora })
        {
        }
    }
}