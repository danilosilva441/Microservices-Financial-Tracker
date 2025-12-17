// Caminho: backend/BillingService/Services/Interfaces/IFaturamentoDiarioService.cs
using BillingService.DTOs;
using BillingService.Models;

namespace BillingService.Services.Interfaces
{
    public interface IFaturamentoDiarioService
    {
        /// <summary>
        /// Usado pelo Líder/Operador para submeter o fechamento do dia.
        /// </summary>
        Task<(FaturamentoDiarioResponseDto? dto, string? errorMessage)> SubmeterFechamentoAsync(
            Guid unidadeId,
            FaturamentoDiarioCreateDto dto,
            Guid userId,
            Guid tenantId);

        /// <summary>
        /// Usado pelo Supervisor para aprovar/rejeitar ou editar um fechamento.
        /// </summary>
        Task<(FaturamentoDiarioResponseDto? dto, string? errorMessage)> RevisarFechamentoAsync(
            Guid faturamentoDiarioId,
            FaturamentoDiarioSupervisorUpdateDto dto,
            Guid supervisorId,
            Guid tenantId);

        /// <summary>
        /// Lista todos os fechamentos de uma unidade.
        /// </summary>
        Task<IEnumerable<FaturamentoDiarioResponseDto>> GetFechamentosPorUnidadeAsync(
            Guid unidadeId,
            Guid tenantId);

        /// <summary>
        /// Lista todos os fechamentos pendentes para um Supervisor (em todo o Tenant).
        /// </summary>
        Task<IEnumerable<FaturamentoDiarioResponseDto>> GetFechamentosPendentesAsync(
            Guid tenantId);

        /// <summary>
        /// Pega um único fechamento pelo ID.
        /// </summary>
        Task<FaturamentoDiarioResponseDto?> GetFechamentoByIdAsync(
            Guid id,
            Guid tenantId);

        Task<IEnumerable<FaturamentoDiario>> GetFechamentosPorDataAsync(
            Guid unidadeId,
            DateOnly dataInicio,
            DateOnly dataFim,
            Guid tenantId);

    }
}