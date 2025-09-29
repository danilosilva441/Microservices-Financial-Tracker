using BillingService.Data;
using BillingService.Models;
using Microsoft.EntityFrameworkCore;

namespace BillingService.Repositories;

public class SolicitacaoRepository
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
        return await _context.SolicitacoesAjuste
            .Include(s => s.Faturamento)
            .ThenInclude(f => f.Operacao)
            .OrderByDescending(s => s.DataSolicitacao)
            .ToListAsync();
    }

    public async Task<SolicitacaoAjuste?> GetByIdComFaturamentoAsync(Guid id)
    {
        return await _context.SolicitacoesAjuste
            .Include(s => s.Faturamento)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}