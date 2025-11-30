using BillingService.DTOs;
using BillingService.Models;

namespace BillingService.Services.Interfaces
{
    public interface IFaturamentoParcialService
    {
        // CORRIGIR: operacaoId → unidadeId
        Task<(FaturamentoParcial? faturamento, string? errorMessage)> AddFaturamentoAsync(
            Guid unidadeId, 
            FaturamentoParcialCreateDto dto, 
            Guid userId, 
            Guid tenantId);
        Task<(bool success, string? errorMessage)> UpdateFaturamentoAsync(
            Guid unidadeId, 
            Guid faturamentoId, 
            FaturamentoParcialUpdateDto dto, 
            Guid userId, 
            Guid tenantId);
        Task<(bool success, string? errorMessage)> DeleteFaturamentoAsync(
            Guid unidadeId, 
            Guid faturamentoId, 
            Guid userId, 
            Guid tenantId);
        Task<(bool success, string? errorMessage)> DeactivateFaturamentoAsync(
            Guid unidadeId, 
            Guid faturamentoId, 
            Guid userId, 
            Guid tenantId);
    }
}