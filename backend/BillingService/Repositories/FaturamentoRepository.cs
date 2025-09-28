using BillingService.Data;
using BillingService.Models;
using Microsoft.EntityFrameworkCore;

namespace BillingService.Repositories;

public class FaturamentoRepository
{
    private readonly BillingDbContext _context;

    public FaturamentoRepository(BillingDbContext context)
    {
        _context = context;
    }

    public async Task<bool> UserHasAccessToOperacaoAsync(Guid operacaoId, Guid userId)
    {
        // Verifica se existe um vínculo na tabela de junção
        return await _context.UsuarioOperacoes
            .AnyAsync(uo => uo.OperacaoId == operacaoId && uo.UserId == userId);
    }

    public async Task<bool> FaturamentoExistsOnDateAsync(Guid operacaoId, DateTime data, Guid? excludeFaturamentoId = null)
    {
        var query = _context.Faturamentos
            .Where(f => f.OperacaoId == operacaoId && f.Data.Date == data.Date);

        if (excludeFaturamentoId.HasValue)
        {
            query = query.Where(f => f.Id != excludeFaturamentoId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<Faturamento?> GetByIdAsync(Guid faturamentoId)
    {
        return await _context.Faturamentos.FindAsync(faturamentoId);
    }

    public async Task AddAsync(Faturamento faturamento)
    {
        await _context.Faturamentos.AddAsync(faturamento);
    }

    public void Update(Faturamento faturamento)
    {
        _context.Faturamentos.Update(faturamento);
    }

    public void Remove(Faturamento faturamento)
    {
        _context.Faturamentos.Remove(faturamento);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}