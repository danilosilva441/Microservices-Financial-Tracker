using BillingService.Data;
using BillingService.Models;
using Microsoft.EntityFrameworkCore;

namespace BillingService.Repositories;

public class EmpresaRepository
{
    private readonly BillingDbContext _context;

    public EmpresaRepository(BillingDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Empresa>> GetAllAsync()
    {
        return await _context.Empresas.ToListAsync();
    }

    public async Task AddAsync(Empresa empresa)
    {
        await _context.Empresas.AddAsync(empresa);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
    public async Task<Empresa?> GetByIdAsync(Guid id)
    {
        return await _context.Empresas.FindAsync(id);
    }

    public void Update(Empresa empresa)
    {
        _context.Empresas.Update(empresa);
    }

    public void Remove(Empresa empresa)
    {
        _context.Empresas.Remove(empresa);
    }
}