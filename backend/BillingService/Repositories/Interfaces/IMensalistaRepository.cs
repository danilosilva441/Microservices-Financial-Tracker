// Caminho: backend/BillingService/Repositories/Interfaces/IMensalistaRepository.cs
using BillingService.Models;

namespace BillingService.Repositories.Interfaces
{
    public interface IMensalistaRepository
    {
        // Métodos copiados da sua classe MensalistaRepository.cs
        Task<IEnumerable<Mensalista>> GetAllByUnidadeIdAsync(Guid unidadeId);
        Task<Mensalista?> GetByIdAsync(Guid mensalistaId);
        Task AddAsync(Mensalista mensalista);
        void Update(Mensalista mensalista);
        Task SaveChangesAsync();
    }
}