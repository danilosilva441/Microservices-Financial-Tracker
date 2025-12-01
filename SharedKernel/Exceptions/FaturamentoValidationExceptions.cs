using System;

namespace SharedKernel.Exceptions
{
    /// <summary>
    /// Exception para DTO de faturamento nulo
    /// </summary>
    public class FaturamentoDtoNullException : BusinessRuleException
    {
        public FaturamentoDtoNullException() 
            : base("DTO não pode ser nulo")
        {
        }
    }

    /// <summary>
    /// Exception para valor inválido no faturamento
    /// </summary>
    public class InvalidFaturamentoValueException : BusinessRuleException
    {
        public InvalidFaturamentoValueException() 
            : base("Valor deve ser maior que zero")
        {
        }

        public InvalidFaturamentoValueException(decimal valor) 
            : base($"Valor {valor} é inválido. Deve ser maior que zero")
        {
        }
    }

    /// <summary>
    /// Exception para origem inválida no faturamento
    /// </summary>
    public class InvalidFaturamentoOriginException : BusinessRuleException
    {
        public InvalidFaturamentoOriginException() 
            : base("Origem é obrigatória")
        {
        }

        public InvalidFaturamentoOriginException(string origem) 
            : base($"Origem '{origem}' é inválida")
        {
        }
    }

    /// <summary>
    /// Exception para período de tempo inválido no faturamento
    /// </summary>
    public class InvalidFaturamentoTimeRangeException : InvalidFaturamentoTimeException
    {
        public InvalidFaturamentoTimeRangeException() 
            : base("Hora fim deve ser posterior à hora início")
        {
        }

        public InvalidFaturamentoTimeRangeException(DateTime inicio, DateTime fim) 
            : base($"Período inválido: {inicio:HH:mm} - {fim:HH:mm}. Hora fim deve ser posterior à hora início")
        {
        }
    }

    /// <summary>
    /// Exception para hora futura no faturamento
    /// </summary>
    public class FutureFaturamentoTimeException : InvalidFaturamentoTimeException
    {
        public FutureFaturamentoTimeException() 
            : base("Hora fim não pode ser futura")
        {
        }

        public FutureFaturamentoTimeException(DateTime horaFim) 
            : base($"Hora fim {horaFim:HH:mm} não pode ser futura")
        {
        }
    }

    /// <summary>
    /// Exception para método de pagamento inválido
    /// </summary>
    public class InvalidPaymentMethodException : BusinessRuleException
    {
        public InvalidPaymentMethodException() 
            : base("Método de pagamento é obrigatório")
        {
        }

        public InvalidPaymentMethodException(Guid metodoPagamentoId) 
            : base($"Método de pagamento {metodoPagamentoId} é inválido")
        {
        }
    }

    /// <summary>
    /// Exception para unidade inválida
    /// </summary>
    public class InvalidUnidadeException : BusinessRuleException
    {
        public InvalidUnidadeException() 
            : base("Unidade é obrigatória")
        {
        }

        public InvalidUnidadeException(Guid unidadeId) 
            : base($"Unidade {unidadeId} é inválida")
        {
        }
    }

    /// <summary>
    /// Exception para data de faturamento inválida
    /// </summary>
    public class InvalidFaturamentoDateException : BusinessRuleException
    {
        public InvalidFaturamentoDateException() 
            : base("Data do faturamento é inválida")
        {
        }

        public InvalidFaturamentoDateException(DateOnly data) 
            : base($"Data {data:dd/MM/yyyy} é inválida para faturamento")
        {
        }

        public InvalidFaturamentoDateException(string message) 
            : base(message)
        {
        }
    }
}