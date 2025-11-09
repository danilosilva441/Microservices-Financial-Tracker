using BillingService.Data;
using BillingService.Models;
using Microsoft.EntityFrameworkCore;

namespace BillingService.Repositories;

public class MetaRepository : Interfaces.IMetaRepository
{
    private readonly BillingDbContext _context;

    public MetaRepository(BillingDbContext context)
    {
        _context = context;
    }

    public async Task<Meta?> GetByUserAndPeriodAsync(Guid userId, int mes, int ano)
    {
        return await _context.Metas
            .FirstOrDefaultAsync(m => m.UserId == userId && m.Mes == mes && m.Ano == ano);
    }

    public async Task AddAsync(Meta meta)
    {
        await _context.Metas.AddAsync(meta);
    }

    public void Update(Meta meta)
    {
        _context.Metas.Update(meta);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}