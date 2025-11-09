// Caminho: backend/BillingService/Repositories/UnidadeRepository.cs
using BillingService.Data;
using BillingService.Models;
using BillingService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BillingService.Repositories;

// 1. Renomeado e implementa a nova interface
public class UnidadeRepository : IUnidadeRepository
{
    private readonly BillingDbContext _context;

    public UnidadeRepository(BillingDbContext context)
    {
        _context = context;
    }

    // 2. MUDANÇA: Usa _context.Unidades e retorna Unidade
    public async Task<Unidade?> GetByIdAsync(Guid id, Guid tenantId)
    {
        return await _context.Unidades
            .FirstOrDefaultAsync(u => u.Id == id && u.TenantId == tenantId);
    }

    // 3. MUDANÇA: Usa _context.Unidades e retorna Unidade
    public async Task<IEnumerable<Unidade>> GetAllAsync(Guid tenantId)
    {
        return await _context.Unidades
            .Where(u => u.TenantId == tenantId)
            .ToListAsync();
    }

    // 4. MUDANÇA: Recebe Unidade
    public async Task AddAsync(Unidade unidade)
    {
        await _context.Unidades.AddAsync(unidade);
    }

    public async Task AddUsuarioOperacaoLinkAsync(UsuarioOperacao vinculo)
    {
        await _context.UsuarioOperacoes.AddAsync(vinculo);
    }

    // 5. MUDANÇA: Recebe Unidade
    public void Update(Unidade unidade)
    {
        _context.Unidades.Update(unidade);
    }

    // 6. MUDANÇA: Recebe Unidade
    public void Remove(Unidade unidade)
    {
        _context.Unidades.Remove(unidade);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
    
    public async Task<bool> UpdateProjecaoAsync(Guid id, decimal projecao)
    {
        // 7. MUDANÇA: Busca em _context.Unidades
        var unidade = await _context.Unidades.FindAsync(id);
        if (unidade == null)
        {
            return false; 
        }

        unidade.ProjecaoFaturamento = projecao;
        await _context.SaveChangesAsync();

        return true; 
    }
}