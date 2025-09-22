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
        return await _context.Operacoes
            .Include(op => op.Faturamentos)
            .Where(op => op.UserId == userId)
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