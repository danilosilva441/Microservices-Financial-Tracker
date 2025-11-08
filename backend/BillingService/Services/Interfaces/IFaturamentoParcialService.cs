// Caminho: backend/BillingService/Services/Interfaces/IFaturamentoParcialService.cs
using BillingService.DTOs; // Vamos criar os DTOs em breve
using BillingService.Models;

namespace BillingService.Services.Interfaces
{
    public interface IFaturamentoParcialService
    {
        // Métodos v2.0 (agora usam TenantId e DTOs v2.0)
        Task<(FaturamentoParcial? faturamento, string? errorMessage)> AddFaturamentoAsync(Guid unidadeId, FaturamentoParcialCreateDto dto, Guid userId, Guid tenantId);
        Task<(bool success, string? errorMessage)> UpdateFaturamentoAsync(Guid unidadeId, Guid faturamentoId, FaturamentoParcialUpdateDto dto, Guid userId, Guid tenantId);
        Task<(bool success, string? errorMessage)> DeleteFaturamentoAsync(Guid unidadeId, Guid faturamentoId, Guid userId, Guid tenantId);
        Task<(bool success, string? errorMessage)> DeactivateFaturamentoAsync(Guid unidadeId, Guid faturamentoId, Guid userId, Guid tenantId);
    }
}