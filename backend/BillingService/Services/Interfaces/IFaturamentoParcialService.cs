using BillingService.DTOs;
using BillingService.Models;

namespace BillingService.Services.Interfaces
{
    public interface IFaturamentoParcialService
    {
        // ✅ Atualizado: Retorna apenas FaturamentoParcial (exceptions tratam os erros)
        Task<FaturamentoParcial> AddFaturamentoAsync(
            Guid unidadeId, 
            FaturamentoParcialCreateDto dto, 
            Guid userId, 
            Guid tenantId);
            
        // ✅ Atualizado: Retorna void (exceptions tratam os erros)
        Task UpdateFaturamentoAsync(
            Guid unidadeId, 
            Guid faturamentoId, 
            FaturamentoParcialUpdateDto dto, 
            Guid userId, 
            Guid tenantId);
            
        // ✅ Atualizado: Retorna void (exceptions tratam os erros)
        Task DeleteFaturamentoAsync(
            Guid unidadeId, 
            Guid faturamentoId, 
            Guid userId, 
            Guid tenantId);
            
        // ✅ Atualizado: Retorna void (exceptions tratam os erros)
        Task DeactivateFaturamentoAsync(
            Guid unidadeId, 
            Guid faturamentoId, 
            Guid userId, 
            Guid tenantId);
            
        // ✅ Método adicional útil (opcional)
        Task<IEnumerable<FaturamentoParcial>> GetFaturamentosPorUnidadeEDataAsync(
            Guid unidadeId, DateOnly data, Guid tenantId);
    }
}