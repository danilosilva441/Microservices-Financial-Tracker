// Caminho: backend/BillingService/Repositories/FaturamentoParcialRepository.cs
using BillingService.Data;
using BillingService.Models;
using BillingService.Repositories.Interfaces; // <-- 1. IMPORTANTE: Adiciona o using
using Microsoft.EntityFrameworkCore;

namespace BillingService.Repositories
{
    public class FaturamentoParcialRepository : IFaturamentoParcialRepository // <-- 2. Herda da Interface
    {
        private readonly BillingDbContext _context;

        public FaturamentoParcialRepository(BillingDbContext context)
        {
            _context = context;
        }

        // 3. CORRIGIDO (v2.0): Renomeado e usa UnidadeId
        public async Task<bool> UserHasAccessToUnidadeAsync(Guid unidadeId, Guid userId, Guid tenantId)
        {
            return await _context.UsuarioOperacoes
                .AnyAsync(uo => uo.TenantId == tenantId && 
                               uo.UnidadeId == unidadeId && // <-- Corrigido (CS1061)
                               uo.UserId == userId);
        }

        // 4. CORRIGIDO (v2.0): Renomeado e usa FaturamentoDiarioId
        public async Task<bool> CheckForOverlappingFaturamentoAsync(Guid faturamentoDiarioId, Guid tenantId, DateTime inicio, DateTime fim, Guid? excludeFaturamentoId = null)
        {
            var query = _context.FaturamentosParciais
                // 5. CORRIGIDO (v2.0)
                .Where(f => f.TenantId == tenantId && f.FaturamentoDiarioId == faturamentoDiarioId); 

            if (excludeFaturamentoId.HasValue)
            {
                query = query.Where(f => f.Id != excludeFaturamentoId.Value);
            }
            
            return await query.AnyAsync(f => f.HoraInicio < fim && f.HoraFim > inicio);
        }

        // (O restante do arquivo está correto)

        public async Task<FaturamentoParcial?> GetByIdAsync(Guid faturamentoId, Guid tenantId)
        {
            return await _context.FaturamentosParciais
                .FirstOrDefaultAsync(f => f.Id == faturamentoId && f.TenantId == tenantId);
        }

        public async Task AddAsync(FaturamentoParcial faturamento)
        {
            await _context.FaturamentosParciais.AddAsync(faturamento);
        }

        public void Update(FaturamentoParcial faturamento)
        {
            _context.FaturamentosParciais.Update(faturamento);
        }

        public void Remove(FaturamentoParcial faturamento)
        {
            _context.FaturamentosParciais.Remove(faturamento);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}