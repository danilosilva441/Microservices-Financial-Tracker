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
}