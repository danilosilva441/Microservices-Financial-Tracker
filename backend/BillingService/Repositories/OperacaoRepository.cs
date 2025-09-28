using BillingService.Data;
using BillingService.Models;
using Microsoft.EntityFrameworkCore;

namespace BillingService.Repositories;

public class OperacaoRepository
{
    private readonly BillingDbContext _context;

    public OperacaoRepository(BillingDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Operacao>> GetByUserIdAsync(Guid userId, int? ano, int? mes, bool? isAtiva)
    {
        // 1. Buscamos os IDs das operações permitidas para o usuário.
        var operacoesIds = await _context.UsuarioOperacoes
            .Where(uo => uo.UserId == userId)
            .Select(uo => uo.OperacaoId)
            .ToListAsync();

        // 2. Criamos a consulta inicial para buscar as operações com seus faturamentos.
        var query = _context.Operacoes
            .Where(op => operacoesIds.Contains(op.Id))
            .Include(op => op.Faturamentos)
            .AsQueryable();

        // 3. Aplicamos os filtros opcionais
        if (isAtiva.HasValue)
        {
            query = query.Where(op => op.IsAtiva == isAtiva.Value);
        }
        if (ano.HasValue)
        {
            query = query.Where(op => op.DataInicio.Year == ano.Value);
        }
        if (mes.HasValue)
        {
            query = query.Where(op => op.DataInicio.Month == mes.Value);
        }

        // 4. Finalmente, executamos a consulta completa.
        return await query.ToListAsync();
    }

    public async Task AddAsync(Operacao operacao)
    {
        await _context.Operacoes.AddAsync(operacao);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<Operacao?> GetByIdAndUserIdAsync(Guid id, Guid userId)
    {
        // Valida o vínculo antes de retornar a operação
        var temAcesso = await _context.UsuarioOperacoes.AnyAsync(uo => uo.UserId == userId && uo.OperacaoId == id);
        if (!temAcesso)
        {
            return null;
        }

        return await _context.Operacoes
            .Include(op => op.Faturamentos)
            .FirstOrDefaultAsync(op => op.Id == id);
    }

    public void Update(Operacao operacao)
    {
        _context.Operacoes.Update(operacao);
    }

    public async Task AddUsuarioOperacaoLinkAsync(UsuarioOperacao vinculo)
    {
        await _context.UsuarioOperacoes.AddAsync(vinculo);
    }

    public void Remove(Operacao operacao)
    {
        _context.Operacoes.Remove(operacao);
    }
    public async Task<IEnumerable<Operacao>> GetAllAsync(int? ano, int? mes, bool? isAtiva)
    {
        var query = _context.Operacoes
            .Include(op => op.Faturamentos)
            .AsQueryable();

        if (isAtiva.HasValue) query = query.Where(op => op.IsAtiva == isAtiva.Value);
        if (ano.HasValue) query = query.Where(op => op.DataInicio.Year == ano.Value);
        if (mes.HasValue) query = query.Where(op => op.DataInicio.Month == mes.Value);

        return await query.ToListAsync();
    }
}