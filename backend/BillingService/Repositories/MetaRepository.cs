// Caminho: backend/BillingService/Repositories/MetaRepository.cs
using BillingService.Data;
using BillingService.Models;
using BillingService.Repositories.Interfaces; // 1. IMPORTANTE
using Microsoft.EntityFrameworkCore;

namespace BillingService.Repositories
{
    public class MetaRepository : IMetaRepository // 2. Herda da interface
    {
        private readonly BillingDbContext _context;

        public MetaRepository(BillingDbContext context)
        {
            _context = context;
        }

        // 3. CORRIGIDO (v2.0): Usa UnidadeId e TenantId
        public async Task<Meta?> GetByUnidadeAndPeriodAsync(Guid unidadeId, int mes, int ano, Guid tenantId)
        {
            return await _context.Metas
                .FirstOrDefaultAsync(m => 
                    m.UnidadeId == unidadeId && 
                    m.Mes == mes && 
                    m.Ano == ano && 
                    m.TenantId == tenantId); // 4. Adiciona TenantId
        }
        
        // 5. NOVO (v2.0): Lista todas as metas de uma unidade
        public async Task<IEnumerable<Meta>> GetAllByUnidadeAsync(Guid unidadeId, Guid tenantId)
        {
             return await _context.Metas
                .Where(m => m.UnidadeId == unidadeId && m.TenantId == tenantId)
                .OrderByDescending(m => m.Ano).ThenByDescending(m => m.Mes)
                .ToListAsync();
        }

        public async Task AddAsync(Meta meta)
        {
            await _context.Metas.AddAsync(meta);
        }

        public void Update(Meta meta)
        {
            _context.Metas.Update(meta);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}