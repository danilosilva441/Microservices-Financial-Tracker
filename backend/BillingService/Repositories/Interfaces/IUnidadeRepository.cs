// Caminho: backend/BillingService/Repositories/Interfaces/IUnidadeRepository.cs
using BillingService.Models;

namespace BillingService.Repositories.Interfaces
{
    // 1. Renomeado de IOperacaoRepository para IUnidadeRepository
    public interface IUnidadeRepository
    {
        // 2. Métodos v2.0 (focados no Tenant)
        Task<Unidade?> GetByIdAsync(Guid id, Guid tenantId);
        Task<IEnumerable<Unidade>> GetAllAsync(Guid tenantId);
        
        // Métodos CRUD
        Task AddAsync(Unidade unidade);
        void Update(Unidade unidade);
        void Remove(Unidade unidade);
        Task SaveChangesAsync();

        // Métodos de Vínculo
        Task AddUsuarioOperacaoLinkAsync(UsuarioOperacao vinculo);
        
        // Método de Projeção
        Task<bool> UpdateProjecaoAsync(Guid id, decimal projecao);
    }
}