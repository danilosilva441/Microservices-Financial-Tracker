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

    public async Task<IEnumerable<Operacao>> GetByUserIdAsync(Guid userId)
    {
        // A consulta agora seleciona as Operacoes onde o Id está na lista
        // de OperacaoId's vinculados ao userId na tabela UsuarioOperacoes.
        return await _context.UsuarioOperacoes
            .Where(uo => uo.UserId == userId)
            .Select(uo => uo.Operacao)
            .Include(op => op.Faturamentos)
            .ToListAsync();
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