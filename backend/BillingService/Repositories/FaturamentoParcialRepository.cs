// Caminho: backend/BillingService/Repositories/FaturamentoParcialRepository.cs
using BillingService.Data;
using BillingService.Models;
using BillingService.Repositories.Interfaces; // 1. IMPORTANTE: Adiciona o using
using Microsoft.EntityFrameworkCore;

namespace BillingService.Repositories
{
    // 2. MUDANÇA: Renomeado e implementa a nova interface v2.0
    public class FaturamentoParcialRepository : IFaturamentoParcialRepository
    {
        private readonly BillingDbContext _context;

        public FaturamentoParcialRepository(BillingDbContext context)
        {
            _context = context;
        }

        // 3. CORRIGIDO (v2.0): Renomeado e usa UnidadeId (vinda do modelo UsuarioOperacao)
        public async Task<bool> UserHasAccessToUnidadeAsync(Guid unidadeId, Guid userId, Guid tenantId)
        {
            return await _context.UsuarioOperacoes
                .AnyAsync(uo => uo.TenantId == tenantId && 
                               uo.UnidadeId == unidadeId && 
                               uo.UserId == userId);
        }

        // 4. CORRIGIDO (v2.0): Renomeado e usa FaturamentoDiarioId (vinda do modelo FaturamentoParcial)
        public async Task<bool> CheckForOverlappingFaturamentoAsync(Guid faturamentoDiarioId, Guid tenantId, DateTime inicio, DateTime fim, Guid? excludeFaturamentoId = null)
        {
            var query = _context.FaturamentosParciais
                .Where(f => f.TenantId == tenantId && f.FaturamentoDiarioId == faturamentoDiarioId);

            if (excludeFaturamentoId.HasValue)
            {
                query = query.Where(f => f.Id != excludeFaturamentoId.Value);
            }
            
            // Lógica de sobreposição
            return await query.AnyAsync(f => f.HoraInicio < fim && f.HoraFim > inicio);
        }

        // 5. CORRIGIDO (v2.0): Adiciona filtro de TenantId por segurança
        public async Task<FaturamentoParcial?> GetByIdAsync(Guid faturamentoId, Guid tenantId)
        {
            return await _context.FaturamentosParciais
                .FirstOrDefaultAsync(f => f.Id == faturamentoId && f.TenantId == tenantId);
        }

        // 6. CORRIGIDO (v2.0): Recebe o novo modelo FaturamentoParcial
        public async Task AddAsync(FaturamentoParcial faturamento)
        {
            await _context.FaturamentosParciais.AddAsync(faturamento);
        }

        // 7. CORRIGIDO (v2.0): Recebe o novo modelo FaturamentoParcial
        public void Update(FaturamentoParcial faturamento)
        {
            _context.FaturamentosParciais.Update(faturamento);
        }

        // 8. CORRIGIDO (v2.0): Recebe o novo modelo FaturamentoParcial
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