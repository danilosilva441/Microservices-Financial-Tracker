using BillingService.DTO;
using BillingService.Models;

namespace BillingService.Services.Interfaces
{
    public interface IFaturamentoParcialService
    {
        // Todos os métodos de serviço agora recebem o TenantId e UserId (do JWT)
        Task<(FaturamentoParcial? faturamento, string? errorMessage)> AddFaturamentoAsync(Guid operacaoId, FaturamentoParcialCreateDto dto, Guid userId, Guid tenantId);
        Task<(bool success, string? errorMessage)> UpdateFaturamentoAsync(Guid operacaoId, Guid faturamentoId, FaturamentoParcialUpdateDto dto, Guid userId, Guid tenantId);
        Task<(bool success, string? errorMessage)> DeleteFaturamentoAsync(Guid operacaoId, Guid faturamentoId, Guid userId, Guid tenantId);
        Task<(bool success, string? errorMessage)> DeactivateFaturamentoAsync(Guid operacaoId, Guid faturamentoId, Guid userId, Guid tenantId);
    }
}