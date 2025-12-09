// SharedKernel/Exceptions/ValidationException.cs
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharedKernel.Exceptions;

/// <summary>
/// Exceção lançada quando ocorrem erros de validação (genérico)
/// </summary>
public class ValidationException : BaseException
{
    /// <summary>
    /// Lista de erros de validação
    /// </summary>
    public IDictionary<string, string[]> Errors { get; }

    /// <summary>
    /// Inicializa uma nova instância da classe ValidationException
    /// </summary>
    /// <param name="errors">Dicionário de erros de validação</param>
    /// <param name="errorCode">Código de erro (padrão: Validation)</param>
    /// <param name="message">Mensagem de erro (opcional)</param>
    /// <param name="innerException">Exceção interna (opcional)</param>
    public ValidationException(
        IDictionary<string, string[]> errors,
        string errorCode = ErrorCodes.RequiredField,
        string? message = null,
        Exception? innerException = null)
        : base(
            errorCode,
            message ?? ErrorMessages.RequiredField,
            ErrorSeverity.Warning,
            innerException,
            new { ValidationErrors = errors })
    {
        Errors = errors ?? throw new ArgumentNullException(nameof(errors));
    }

    /// <summary>
    /// Inicializa uma nova instância da classe ValidationException
    /// </summary>
    /// <param name="propertyName">Nome da propriedade</param>
    /// <param name="errorMessage">Mensagem de erro</param>
    /// <param name="errorCode">Código de erro específico</param>
    /// <param name="message">Mensagem geral (opcional)</param>
    /// <param name="innerException">Exceção interna (opcional)</param>
    public ValidationException(
        string propertyName,
        string errorMessage,
        string errorCode = ErrorCodes.RequiredField,
        string? message = null,
        Exception? innerException = null)
        : this(
            new Dictionary<string, string[]> { [propertyName] = new[] { errorMessage } },
            errorCode,
            message,
            innerException)
    {
    }

    /// <summary>
    /// Cria uma ValidationException para um campo obrigatório
    /// </summary>
    public static ValidationException RequiredField(string propertyName)
    {
        var message = string.Format(ErrorMessages.RequiredField, propertyName);
        return new ValidationException(
            propertyName,
            message,
            ErrorCodes.RequiredField);
    }

    /// <summary>
    /// Cria uma ValidationException para limite de caracteres
    /// </summary>
    public static ValidationException StringLengthExceeded(
        string propertyName, 
        int maxLength)
    {
        var message = string.Format(
            ErrorMessages.StringLengthExceeded, 
            propertyName, 
            maxLength);
        return new ValidationException(
            propertyName,
            message,
            ErrorCodes.StringLengthExceeded);
    }

    /// <summary>
    /// Cria uma ValidationException para formato de email inválido
    /// </summary>
    public static ValidationException InvalidEmail(string propertyName = "Email")
    {
        return new ValidationException(
            propertyName,
            ErrorMessages.InvalidEmailFormat,
            ErrorCodes.InvalidEmailFormat);
    }

    /// <summary>
    /// Formata os erros em uma string legível
    /// </summary>
    public string GetFormattedErrors()
    {
        return string.Join(Environment.NewLine,
            Errors.SelectMany(kvp =>
                kvp.Value.Select(error => $"{kvp.Key}: {error}")));
    }
}