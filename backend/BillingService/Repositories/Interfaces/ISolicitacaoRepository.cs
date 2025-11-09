// Caminho: backend/BillingService/Repositories/Interfaces/ISolicitacaoRepository.cs
using BillingService.Models;

namespace BillingService.Repositories.Interfaces
{
    public interface ISolicitacaoRepository
    {
        // Métodos copiados da sua classe SolicitacaoRepository.cs
        Task AddAsync(SolicitacaoAjuste solicitacao);
        Task<List<SolicitacaoAjuste>> GetAllComDetalhesAsync();
        Task<SolicitacaoAjuste?> GetByIdComFaturamentoAsync(Guid id);
        Task SaveChangesAsync();
    }
}