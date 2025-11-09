// Caminho: backend/BillingService/Repositories/Interfaces/IFaturamentoDiarioRepository.cs
using BillingService.Models;

namespace BillingService.Repositories.Interfaces
{
    public interface IFaturamentoDiarioRepository
    {
        Task AddAsync(FaturamentoDiario faturamentoDiario);
        void Update(FaturamentoDiario faturamentoDiario);
        
        /// <summary>
        /// Busca um FaturamentoDiario pelo ID, verificando o Tenant.
        /// </summary>
        Task<FaturamentoDiario?> GetByIdAsync(Guid id, Guid tenantId);

        /// <summary>
        /// Busca um FaturamentoDiario pela Unidade e Data.
        /// </summary>
        Task<FaturamentoDiario?> GetByUnidadeAndDateAsync(Guid unidadeId, DateOnly data, Guid tenantId);

        /// <summary>
        /// Lista todos os FaturamentosDiarios de uma Unidade, incluindo os Parciais.
        /// </summary>
        Task<IEnumerable<FaturamentoDiario>> ListByUnidadeAsync(Guid unidadeId, Guid tenantId);

        /// <summary>
        /// Lista FaturamentosDiarios por Status (ex: todos "Pendentes") para um Tenant.
        /// </summary>
        Task<IEnumerable<FaturamentoDiario>> ListByStatusAsync(RegistroStatus status, Guid tenantId);
        
        Task SaveChangesAsync();
    }
}