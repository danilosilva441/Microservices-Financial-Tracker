using System;
using SharedKernel;
using SharedKernel.Exceptions;

namespace BillingService.Services.Exceptions  // Mudei o namespace!
{
    /// <summary>
    /// Exception para DTO de faturamento nulo
    /// </summary>
    public class FaturamentoDtoNullException : ValidationException
    {
        public FaturamentoDtoNullException()
            : base(
                "FaturamentoDto",
                ErrorMessages.RequiredField,
                ErrorCodes.RequiredField,
                "O objeto de faturamento é obrigatório.")
        {
        }

        public FaturamentoDtoNullException(string propertyName)
            : base(
                propertyName,
                "O DTO de faturamento não pode ser nulo.",
                ErrorCodes.RequiredField,
                "O objeto de faturamento é obrigatório.")
        {
        }
    }

    /// <summary>
    /// Exception para valor inválido no faturamento
    /// </summary>
    public class InvalidFaturamentoValueException : ValidationException
    {
        public InvalidFaturamentoValueException()
            : base(
                "Valor",
                "Valor do faturamento deve ser maior que zero",
                ErrorCodes.InvalidNumberRange,
                "Valor do faturamento inválido.")
        {
        }

        public InvalidFaturamentoValueException(decimal valor)
            : base(
                "Valor",
                $"Valor {valor} é inválido. Deve ser maior que zero",
                ErrorCodes.InvalidNumberRange,
                $"Valor {valor} é inválido para faturamento.")
        {
        }

        public InvalidFaturamentoValueException(decimal valor, decimal minimo, decimal maximo)
            : base(
                "Valor",
                $"Valor {valor} deve estar entre {minimo} e {maximo}",
                ErrorCodes.InvalidNumberRange,
                $"Valor do faturamento fora do intervalo permitido.")
        {
        }
    }

    /// <summary>
    /// Exception para origem inválida no faturamento
    /// </summary>
    public class InvalidFaturamentoOriginException : ValidationException
    {
        public InvalidFaturamentoOriginException()
            : base(
                "Origem",
                "Origem do faturamento é obrigatória",
                ErrorCodes.RequiredField,
                "Origem do faturamento é obrigatória")
        {
        }

        public InvalidFaturamentoOriginException(string origem)
            : base(
                "Origem",
                $"Origem '{origem}' é inválida",
                ErrorCodes.InvalidOperation,
                $"Origem '{origem}' não é permitida para faturamento.")
        {
        }

        public InvalidFaturamentoOriginException(string origem, string[] origensValidas)
            : base(
                propertyName: "Origem",
                errorMessage: $"Origem '{origem}' é inválida. Valores válidos: {string.Join(", ", origensValidas)}",
                errorCode: ErrorCodes.InvalidOperation,
                message: "Origem do faturamento inválida.")
        {
            // Você pode armazenar os dados adicionais de outra forma se necessário
        }
    }

    /// <summary>
    /// Exception para período de tempo inválido no faturamento
    /// </summary>
    public class InvalidFaturamentoTimeRangeException : BusinessException
    {
        public InvalidFaturamentoTimeRangeException()
            : base(
                "INVALID_TIME_RANGE",
                ErrorCodes.InvalidDateRange,
                ErrorMessages.InvalidDateRange)
        {
        }

        public InvalidFaturamentoTimeRangeException(DateTime inicio, DateTime fim)
            : base(
                "INVALID_TIME_RANGE",
                ErrorCodes.InvalidDateRange,
                $"Período inválido: {inicio:HH:mm} - {fim:HH:mm}. Hora fim deve ser posterior à hora início",
                additionalData: new { Inicio = inicio, Fim = fim })
        {
        }
    }

    /// <summary>
    /// Exception para hora futura no faturamento
    /// </summary>
    public class FutureFaturamentoTimeException : BusinessException
    {
        public FutureFaturamentoTimeException()
            : base(
                "FUTURE_TIME",
                ErrorCodes.DataFuturaNaoPermitida,
                ErrorMessages.DataFuturaNaoPermitida)
        {
        }

        public FutureFaturamentoTimeException(DateTime horaFim)
            : base(
                "FUTURE_TIME",
                ErrorCodes.DataFuturaNaoPermitida,
                $"Hora fim {horaFim:HH:mm} não pode ser futura",
                additionalData: new { HoraFim = horaFim })
        {
        }
    }

    /// <summary>
    /// Exception para método de pagamento inválido
    /// </summary>
    public class InvalidPaymentMethodException : NotFoundException
    {
        public InvalidPaymentMethodException()
            : base(
                "MetodoPagamento",
                null,
                ErrorCodes.GenericNotFound,
                "Método de pagamento é obrigatório")
        {
        }

        public InvalidPaymentMethodException(Guid metodoPagamentoId)
            : base(
                "MetodoPagamento",
                metodoPagamentoId,
                ErrorCodes.GenericNotFound,
                $"Método de pagamento '{metodoPagamentoId}' é inválido ou não encontrado")
        {
        }
    }

    /// <summary>
    /// Exception para unidade inválida
    /// </summary>
    public class InvalidUnidadeException : NotFoundException
    {
        public InvalidUnidadeException()
            : base(
                "Unidade",
                null,
                ErrorCodes.UnidadeNotFound,
                "Unidade é obrigatória")
        {
        }

        public InvalidUnidadeException(Guid unidadeId)
            : base(
                "Unidade",
                unidadeId,
                ErrorCodes.UnidadeNotFound,
                $"Unidade '{unidadeId}' é inválida ou não encontrada")
        {
        }
    }

    /// <summary>
    /// Exception para data de faturamento inválida
    /// </summary>
    public class InvalidFaturamentoDateException : BusinessException
    {
        public InvalidFaturamentoDateException()
            : base(
                "INVALID_DATE",
                ErrorCodes.InvalidDateFormat,
                ErrorMessages.InvalidDateFormat)
        {
        }

        public InvalidFaturamentoDateException(DateOnly data)
            : base(
                "INVALID_DATE",
                ErrorCodes.InvalidDateFormat,
                $"Data {data:dd/MM/yyyy} é inválida para faturamento",
                additionalData: new { Data = data })
        {
        }

        public InvalidFaturamentoDateException(string message)
            : base(
                "INVALID_DATE",
                ErrorCodes.InvalidDateFormat,
                message)
        {
        }
    }

    /// <summary>
    /// Exception para faturamento já existente
    /// </summary>
    public class FaturamentoAlreadyExistsException : BusinessException
    {
        public FaturamentoAlreadyExistsException()
            : base(
                "FATURAMENTO_ALREADY_EXISTS",
                ErrorCodes.FaturamentoJaExiste,
                ErrorMessages.FaturamentoJaExiste)
        {
        }

        public FaturamentoAlreadyExistsException(DateOnly data, TimeSpan? horario = null)
            : base(
                "FATURAMENTO_ALREADY_EXISTS",
                ErrorCodes.FaturamentoJaExiste,
                horario.HasValue
                    ? $"Já existe um faturamento registrado para {data:dd/MM/yyyy} às {horario.Value:hh\\:mm}"
                    : $"Já existe um faturamento registrado para {data:dd/MM/yyyy}",
                additionalData: new { Data = data, Horario = horario })
        {
        }
    }
}