// SharedKernel/Exceptions/ConfigurationException.cs
using System;

namespace SharedKernel.Exceptions;

/// <summary>
/// Exceção lançada quando há problemas de configuração
/// </summary>
public class ConfigurationException : BaseException
{
    /// <summary>
    /// Nome da configuração faltante ou inválida
    /// </summary>
    public string ConfigurationKey { get; }

    public ConfigurationException(
        string configurationKey,
        string? message = null,
        Exception? innerException = null)
        : base(
            message ?? $"Configuração inválida ou faltante: {configurationKey}",
            "CONFIGURATION_ERROR",
            ErrorSeverity.Critical,
            innerException,
            new { ConfigurationKey = configurationKey })
    {
        ConfigurationKey = configurationKey;
    }
}