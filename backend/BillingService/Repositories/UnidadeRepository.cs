// Caminho: backend/BillingService/Repositories/UnidadeRepository.cs
using BillingService.Data;
using BillingService.Models;
using BillingService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BillingService.Repositories;

// 1. Renomeado e implementa a nova interface v2.0
public class UnidadeRepository : IUnidadeRepository
{
    private readonly BillingDbContext _context;

    public UnidadeRepository(BillingDbContext context)
    {
        _context = context;
    }

    // --- MÉTODOS v2.0 (Implementados corretamente) ---

    // 2. Novo método (v2.0) que usa TenantId
    public async Task<Unidade?> GetByIdAsync(Guid id, Guid tenantId)
    {
        return await _context.Unidades
            .FirstOrDefaultAsync(u => u.Id == id && u.TenantId == tenantId);
    }

    // 3. Novo método (v2.0) que usa TenantId
    public async Task<IEnumerable<Unidade>> GetAllAsync(Guid tenantId)
    {
        return await _context.Unidades
            .Where(u => u.TenantId == tenantId)
            .ToListAsync();
    }

    // --- MÉTODOS ANTIGOS v1.0 REMOVIDOS ---
    // GetByUserIdAsync, GetAllAsync (antigo) e GetByIdAndUserIdAsync
    // foram REMOVIDOS, pois usavam a lógica v1.0 (ex: op.Faturamentos)
    // que não existe mais.

    // --- MÉTODOS CRUD (Atualizados para Unidade) ---

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