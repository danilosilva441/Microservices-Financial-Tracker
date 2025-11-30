using AuthService.Models;

namespace AuthService.Repositories
{
    // Contrato para operações de dados do Tenant
    public interface ITenantRepository
    {
        Task AddTenantAsync(Tenant tenant);
        Task<Tenant?> GetByIdAsync(Guid id);
        Task<Tenant?> GetByIdWithUsersAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task UpdateAsync(Tenant tenant);
        Task<List<Tenant>> GetAllAsync();
        Task<List<Tenant>> GetByStatusAsync(string status);
        Task<int> GetUserCountAsync(Guid tenantId);
        Task<bool> IsNameUniqueAsync(string nomeDaEmpresa, Guid? excludeTenantId = null);
    }
}