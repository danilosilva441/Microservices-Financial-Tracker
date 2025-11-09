// Caminho: backend/BillingService/Repositories/Interfaces/IMetaRepository.cs
using BillingService.Models;

namespace BillingService.Repositories.Interfaces
{
    public interface IMetaRepository
    {
        // --- MÉTODOS v2.0 ---
        // Agora busca por Unidade e Tenant, não por Usuário
        Task<Meta?> GetByUnidadeAndPeriodAsync(Guid unidadeId, int mes, int ano, Guid tenantId);
        Task<IEnumerable<Meta>> GetAllByUnidadeAsync(Guid unidadeId, Guid tenantId);
        
        Task AddAsync(Meta meta);
        void Update(Meta meta);
        Task SaveChangesAsync();
    }
}