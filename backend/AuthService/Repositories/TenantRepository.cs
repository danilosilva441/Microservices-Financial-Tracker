using AuthService.Data;
using AuthService.Models;
using Microsoft.EntityFrameworkCore;

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
            await _context.Tenants.AddAsync(tenant);
            // Não chama SaveChangesAsync() aqui para manter o controle da transação no service
        }

        public async Task<Tenant?> GetByIdAsync(Guid id)
        {
            return await _context.Tenants
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Tenant?> GetByIdWithUsersAsync(Guid id)
        {
            return await _context.Tenants
                .AsNoTracking()
                .Include(t => t.Users)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Tenants
                .AsNoTracking()
                .AnyAsync(t => t.Id == id);
        }

        public async Task UpdateAsync(Tenant tenant)
        {
            // CORREÇÃO: Adicionar await para evitar warning
            await Task.Run(() => 
            {
                _context.Tenants.Update(tenant);
            });
            // SaveChangesAsync() deve ser chamado pelo service para controle transacional
        }

        public async Task<List<Tenant>> GetAllAsync()
        {
            return await _context.Tenants
                .AsNoTracking()
                .OrderBy(t => t.NomeDaEmpresa)
                .ToListAsync();
        }

        public async Task<List<Tenant>> GetByStatusAsync(string status)
        {
            return await _context.Tenants
                .AsNoTracking()
                .Where(t => t.StatusDaSubscricao == status)
                .OrderBy(t => t.NomeDaEmpresa)
                .ToListAsync();
        }

        public async Task<int> GetUserCountAsync(Guid tenantId)
        {
            return await _context.Users
                .AsNoTracking()
                .CountAsync(u => u.TenantId == tenantId);
        }

        public async Task<bool> IsNameUniqueAsync(string nomeDaEmpresa, Guid? excludeTenantId = null)
        {
            var query = _context.Tenants
                .Where(t => t.NomeDaEmpresa.ToLower() == nomeDaEmpresa.Trim().ToLower());

            if (excludeTenantId.HasValue)
            {
                query = query.Where(t => t.Id != excludeTenantId.Value);
            }

            return !await query.AnyAsync();
        }
    }
}