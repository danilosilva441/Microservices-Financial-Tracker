// Caminho: backend/BillingService/Repositories/FaturamentoDiarioRepository.cs
using BillingService.Data;
using BillingService.Models;
using BillingService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BillingService.Repositories
{
    public class FaturamentoDiarioRepository : IFaturamentoDiarioRepository
    {
        private readonly BillingDbContext _context;

        public FaturamentoDiarioRepository(BillingDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(FaturamentoDiario faturamentoDiario)
        {
            await _context.FaturamentosDiarios.AddAsync(faturamentoDiario);
        }

        public void Update(FaturamentoDiario faturamentoDiario)
        {
            _context.FaturamentosDiarios.Update(faturamentoDiario);
        }

        public async Task<FaturamentoDiario?> GetByIdAsync(Guid id, Guid tenantId)
        {
            return await _context.FaturamentosDiarios
                .Include(fd => fd.FaturamentosParciais) // Inclui os "itens"
                .FirstOrDefaultAsync(fd => fd.Id == id && fd.TenantId == tenantId);
        }

        public async Task<FaturamentoDiario?> GetByUnidadeAndDateAsync(Guid unidadeId, DateOnly data, Guid tenantId)
        {
            return await _context.FaturamentosDiarios
                .Include(fd => fd.FaturamentosParciais)
                .FirstOrDefaultAsync(fd => fd.UnidadeId == unidadeId && fd.Data == data && fd.TenantId == tenantId);
        }

        public async Task<IEnumerable<FaturamentoDiario>> ListByUnidadeAsync(Guid unidadeId, Guid tenantId)
        {
            return await _context.FaturamentosDiarios
                .Where(fd => fd.UnidadeId == unidadeId && fd.TenantId == tenantId)
                .Include(fd => fd.FaturamentosParciais)
                .OrderByDescending(fd => fd.Data)
                .ToListAsync();
        }

        public async Task<IEnumerable<FaturamentoDiario>> ListByStatusAsync(RegistroStatus status, Guid tenantId)
        {
            return await _context.FaturamentosDiarios
                .Where(fd => fd.Status == status && fd.TenantId == tenantId)
                .Include(fd => fd.Unidade) // Inclui o nome da Unidade
                .OrderBy(fd => fd.Data)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}