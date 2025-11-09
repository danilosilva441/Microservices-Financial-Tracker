using BillingService.Data;
using BillingService.Models;
using BillingService.Repositories.Interfaces; // 1. IMPORTANTE: Adiciona o using
using Microsoft.EntityFrameworkCore;

namespace BillingService.Repositories;

public class MensalistaRepository : IMensalistaRepository // 2. Herda da Interface
{
    private readonly BillingDbContext _context;

    public MensalistaRepository(BillingDbContext context)
    {
        _context = context;
    }

    // 3. CORRIGIDO (v2.0): Renomeado o método e o parâmetro
    public async Task<IEnumerable<Mensalista>> GetAllByUnidadeIdAsync(Guid unidadeId)
    {
        return await _context.Mensalistas
            // 4. CORRIGIDO (v2.0): Usa a propriedade UnidadeId
            .Where(m => m.UnidadeId == unidadeId) 
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