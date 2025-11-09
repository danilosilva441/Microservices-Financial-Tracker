// Caminho: backend/BillingService/Repositories/Interfaces/IFaturamentoParcialRepository.cs
using BillingService.Models;

namespace BillingService.Repositories.Interfaces
{
    public interface IFaturamentoParcialRepository
    {
        // 1. MUDANÇA (v2.0): Renomeado de Operacao para Unidade
        Task<bool> UserHasAccessToUnidadeAsync(Guid unidadeId, Guid userId, Guid tenantId);
        
        // 2. MUDANÇA (v2.0): Agora checa por FaturamentoDiarioId
        Task<bool> CheckForOverlappingFaturamentoAsync(Guid faturamentoDiarioId, Guid tenantId, DateTime inicio, DateTime fim, Guid? excludeFaturamentoId = null);
        
        // (Métodos CRUD permanecem corretos)
        Task<FaturamentoParcial?> GetByIdAsync(Guid faturamentoId, Guid tenantId);
        Task AddAsync(FaturamentoParcial faturamento);
        void Update(FaturamentoParcial faturamento);
        void Remove(FaturamentoParcial faturamento);
        Task SaveChangesAsync();
    }
}