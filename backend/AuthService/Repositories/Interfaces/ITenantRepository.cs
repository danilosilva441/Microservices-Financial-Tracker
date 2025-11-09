using AuthService.Models;

namespace AuthService.Repositories
{
    // Contrato para operações de dados do Tenant
    public interface ITenantRepository
    {
        Task AddTenantAsync(Tenant tenant);
        // Futuramente podemos adicionar: Task<Tenant?> GetByIdAsync(Guid id);
    }
}