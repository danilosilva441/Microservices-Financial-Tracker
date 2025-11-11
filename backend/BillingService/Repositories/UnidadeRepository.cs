// Caminho: backend/BillingService/Repositories/UnidadeRepository.cs
using BillingService.Data;
using BillingService.Models;
using BillingService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BillingService.Repositories;

public class UnidadeRepository : IUnidadeRepository
{
    private readonly BillingDbContext _context;

    public UnidadeRepository(BillingDbContext context)
    {
        _context = context;
    }

    // --- Métodos v2.0 (Para Gerentes) ---

    public async Task<Unidade?> GetByIdAsync(Guid id, Guid tenantId)
    {
        return await _context.Unidades
            .FirstOrDefaultAsync(u => u.Id == id && u.TenantId == tenantId);
    }
    
    public async Task<IEnumerable<Unidade>> GetAllAsync(Guid tenantId)
    {
        return await _context.Unidades
            .Where(u => u.TenantId == tenantId)
            .ToListAsync();
    }

    // --- NOVO MÉTODO v2.0 (Para Admin/Sistema) ---
    // (Implementação do método que adicionamos à interface)
    public async Task<IEnumerable<Unidade>> GetAllAdminAsync()
    {
        // Busca TODAS as unidades, sem filtro de Tenant
        return await _context.Unidades.ToListAsync();
    }

    // --- Métodos CRUD (Restantes) ---

    public async Task AddAsync(Unidade unidade)
    {
        await _context.Unidades.AddAsync(unidade);
    }

    public async Task AddUsuarioOperacaoLinkAsync(UsuarioOperacao vinculo)
    {
        await _context.UsuarioOperacoes.AddAsync(vinculo);
    }
    
    public void Update(Unidade unidade)
    {
        _context.Unidades.Update(unidade);
    }
    
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