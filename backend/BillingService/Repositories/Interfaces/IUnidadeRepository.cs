// Caminho: backend/BillingService/Repositories/Interfaces/IUnidadeRepository.cs
using BillingService.Models;

namespace BillingService.Repositories.Interfaces
{
    // 1. Renomeado de IOperacaoRepository para IUnidadeRepository
    public interface IUnidadeRepository
    {
        // 2. Todos os métodos agora usam a classe "Unidade"
        Task<Unidade?> GetByIdAsync(Guid id, Guid tenantId);
        Task<IEnumerable<Unidade>> GetAllAsync(Guid tenantId);
        Task AddAsync(Unidade unidade);
        void Update(Unidade unidade);
        void Remove(Unidade unidade);
        Task SaveChangesAsync();

        // (Estes métodos de vínculo e projeção permanecem)
        Task AddUsuarioOperacaoLinkAsync(UsuarioOperacao vinculo);
        Task<bool> UpdateProjecaoAsync(Guid id, decimal projecao);
    }
}