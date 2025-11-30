// Caminho: backend/BillingService/Services/Interfaces/IUnidadeService.cs
using BillingService.DTOs; // 1. IMPORTANTE: Usando a pasta DTOs
using BillingService.Models;

namespace BillingService.Services.Interfaces
{
    public interface IUnidadeService
    {
        // --- Métodos v2.0 (Separados por Papel) ---
        Task<IEnumerable<Unidade>> GetAllUnidadesByTenantAsync(Guid tenantId);
        Task<IEnumerable<Unidade>> GetAllUnidadesAdminAsync();

        // 2. MUDANÇA: Corrigido de OperacaoDto para UnidadeDto
        Task<Unidade> CreateUnidadeAsync(UnidadeDto unidadeDto, Guid userId, Guid tenantId); 
        
        Task<Unidade?> GetUnidadeByIdAsync(
            Guid id, 
            Guid tenantId);

        // 3. MUDANÇA: Corrigido de UpdateOperacaoDto para UpdateUnidadeDto
        Task<bool> UpdateUnidadeAsync(
            Guid id, 
            UpdateUnidadeDto unidadeDto, 
            Guid tenantId); 

        Task<bool> DeactivateUnidadeAsync(
            Guid id, 
            Guid tenantId);
        Task<bool> DeleteUnidadeAsync(
            Guid id, 
            Guid tenantId);
        Task<bool> UpdateProjecaoAsync(
            Guid id, 
            decimal projecao);
    }
}