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
        // 1. Primeiro, buscamos apenas os IDs das operações permitidas para o usuário.
        var operacoesIds = await _context.UsuarioOperacoes
            .Where(uo => uo.UserId == userId)
            .Select(uo => uo.OperacaoId)
            .ToListAsync();

        // 2. Agora, começamos uma nova consulta na tabela de Operacoes.
        var query = _context.Operacoes
            .Include(op => op.Faturamentos) // Aplicamos o Include primeiro
            .Where(op => operacoesIds.Contains(op.Id)); // Filtramos pelos IDs permitidos

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
        return await _context.Operacoes
            .Include(op => op.Faturamentos)
            .FirstOrDefaultAsync(op => op.Id == id && op.UserId == userId);
    }

    public void Update(Operacao operacao)
    {
        // O EF Core rastreia a entidade, então apenas marcá-la como modificada é suficiente
        _context.Operacoes.Update(operacao);
    }
}