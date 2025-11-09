using BillingService.Data;
using BillingService.Models;
using BillingService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BillingService.Repositories;

public class SolicitacaoRepository : ISolicitacaoRepository 
{
    private readonly BillingDbContext _context;

    public SolicitacaoRepository(BillingDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(SolicitacaoAjuste solicitacao)
    {
        await _context.SolicitacoesAjuste.AddAsync(solicitacao);
    }

    public async Task<List<SolicitacaoAjuste>> GetAllComDetalhesAsync()
    {
        // --- 1. CORRIGIDO (v2.0) ---
        // Descomentado e atualizado para usar a nova relação
        return await _context.SolicitacoesAjuste
            .Include(s => s.FaturamentoParcial) // <-- Corrigido
            .ThenInclude(fp => fp.FaturamentoDiario) // <-- Corrigido (Buscando o "cabeçalho")
                .ThenInclude(fd => fd.Unidade) // <-- Corrigido (Buscando a "unidade")
            .OrderByDescending(s => s.DataSolicitacao)
            .ToListAsync();
    }

    public async Task<SolicitacaoAjuste?> GetByIdComFaturamentoAsync(Guid id)
    {
        // --- 2. CORRIGIDO (v2.0) ---
        return await _context.SolicitacoesAjuste
            .Include(s => s.FaturamentoParcial) // <-- Corrigido
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}