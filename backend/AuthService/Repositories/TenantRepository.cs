using AuthService.Data;
using AuthService.Models;

namespace AuthService.Repositories
{
    public class TenantRepository : ITenantRepository
    {
        private readonly AuthDbContext _context;

        public TenantRepository(AuthDbContext context)
        {
            _context = context;
        }

        public async Task AddTenantAsync(Tenant tenant)
        {
            // Apenas adiciona ao contexto.
            // O SaveChangesAsync() será chamado pelo AuthService
            // para garantir uma transação atômica.
            await _context.Tenants.AddAsync(tenant);
        }
    }
}