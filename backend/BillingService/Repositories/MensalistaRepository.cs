using BillingService.Data;
using BillingService.Models;
using Microsoft.EntityFrameworkCore;

namespace BillingService.Repositories;

public class MensalistaRepository
{
    private readonly BillingDbContext _context;

    public MensalistaRepository(BillingDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Mensalista>> GetAllByOperacaoIdAsync(Guid operacaoId)
    {
        return await _context.Mensalistas
            .Where(m => m.OperacaoId == operacaoId)
            .ToListAsync();
    }

    public async Task<Mensalista?> GetByIdAsync(Guid mensalistaId)
    {
        return await _context.Mensalistas.FindAsync(mensalistaId);
    }

    public async Task AddAsync(Mensalista mensalista)
    {
        await _context.Mensalistas.AddAsync(mensalista);
    }

    public void Update(Mensalista mensalista)
    {
        _context.Mensalistas.Update(mensalista);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}