using BillingService.DTO;
using BillingService.Models;
using BillingService.Repositories;

namespace BillingService.Services;

public class EmpresaService
{
    private readonly EmpresaRepository _repository;

    public EmpresaService(EmpresaRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Empresa>> GetAllEmpresasAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Empresa> CreateEmpresaAsync(CreateEmpresaDto empresaDto)
    {
        var novaEmpresa = new Empresa
        {
            Id = Guid.NewGuid(),
            Nome = empresaDto.Nome,
            CNPJ = empresaDto.CNPJ,
            DiaVencimentoBoleto = empresaDto.DiaVencimentoBoleto
        };

        await _repository.AddAsync(novaEmpresa);
        await _repository.SaveChangesAsync();
        return novaEmpresa;
    }
    public async Task<Empresa?> UpdateEmpresaAsync(Guid id, UpdateEmpresaDto empresaDto)
    {
        var empresaExistente = await _repository.GetByIdAsync(id);
        if (empresaExistente == null) return null;

        empresaExistente.Nome = empresaDto.Nome;
        empresaExistente.CNPJ = empresaDto.CNPJ;
        empresaExistente.DiaVencimentoBoleto = empresaDto.DiaVencimentoBoleto;

        _repository.Update(empresaExistente);
        await _repository.SaveChangesAsync();
        return empresaExistente;
    }

    public async Task<bool> DeleteEmpresaAsync(Guid id)
    {
        var empresaExistente = await _repository.GetByIdAsync(id);
        if (empresaExistente == null) return false;

        _repository.Remove(empresaExistente);
        await _repository.SaveChangesAsync();
        return true;
    }
}