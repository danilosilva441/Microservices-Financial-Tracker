// Caminho: backend/BillingService/Repositories/FaturamentoDiarioRepository.cs
using BillingService.Data;
using BillingService.Models;
using Microsoft.EntityFrameworkCore;
using BillingService.Repositories.Interfaces;

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
            // Este método está CORRETO
            return await _context.FaturamentosDiarios
                .Include(fd => fd.FaturamentosParciais) // Inclui os "itens"
                .FirstOrDefaultAsync(fd => fd.Id == id && fd.TenantId == tenantId);
        }

        public async Task<FaturamentoDiario?> GetByUnidadeAndDateAsync(Guid unidadeId, DateOnly data, Guid tenantId)
        {
            // Este método está CORRETO
            return await _context.FaturamentosDiarios
                .Include(fd => fd.FaturamentosParciais)
                .FirstOrDefaultAsync(fd => fd.UnidadeId == unidadeId && fd.Data == data && fd.TenantId == tenantId);
        }

        public async Task<IEnumerable<FaturamentoDiario>> ListByUnidadeAsync(Guid unidadeId, Guid tenantId)
        {
            // Este método está CORRETO (e é usado pelo AnalysisService)
            return await _context.FaturamentosDiarios
                .Where(fd => fd.UnidadeId == unidadeId && fd.TenantId == tenantId)
                .Include(fd => fd.FaturamentosParciais)
                .OrderByDescending(fd => fd.Data)
                .ToListAsync();
        }

        // --- A CORREÇÃO ESTÁ AQUI ---
        public async Task<IEnumerable<FaturamentoDiario>> ListByStatusAsync(RegistroStatus status, Guid tenantId)
        {
            return await _context.FaturamentosDiarios
                .Where(fd => fd.Status == status && fd.TenantId == tenantId)
                .Include(fd => fd.Unidade) // Inclui o nome da Unidade
                .Include(fd => fd.FaturamentosParciais) // 1. CORREÇÃO: Adiciona os "itens"
                .OrderBy(fd => fd.Data)
                .ToListAsync();
        }
        // --- FIM DA CORREÇÃO ---

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<FaturamentoDiario>> GetAllByUnidadeAsync(Guid tenantId, Guid unidadeId)
        {
            return await _context.FaturamentosDiarios
                .Where(f => f.TenantId == tenantId && f.UnidadeId == unidadeId)
                .ToListAsync();
        }

        public async Task<IEnumerable<FaturamentoDiario>> GetByDateRangeAsync(
                    Guid tenantId,
                    Guid unidadeId,
                    DateOnly dataInicio,
                    DateOnly dataFim)
        {
            return await _context.FaturamentosDiarios
                .Where(f => f.TenantId == tenantId
                    && f.UnidadeId == unidadeId
                    && f.Data >= dataInicio  // Agora ambos são DateOnly
                    && f.Data <= dataFim)     // Agora ambos são DateOnly
                .OrderBy(f => f.Data)
                .ToListAsync();
        }

        public Task<IEnumerable<FaturamentoDiario>> GetByDateRangeAsync(Guid tenantId, Guid unidadeId, DateTime dataInicio, DateTime dataFim)
        {
            throw new NotImplementedException();
        }
    }
}