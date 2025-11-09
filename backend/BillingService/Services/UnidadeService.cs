// Caminho: backend/BillingService/Services/UnidadeService.cs
using BillingService.DTO;
using BillingService.Models;
using BillingService.Repositories.Interfaces;
using BillingService.Services.Interfaces;

namespace BillingService.Services;

// 1. Renomeado e implementa a nova interface
public class UnidadeService : IUnidadeService
{
    // 2. Injeta a nova interface
    private readonly IUnidadeRepository _repository;

    public UnidadeService(IUnidadeRepository repository) // <-- MUDANÇA AQUI
    {
        _repository = repository;
    }

    // 3. Renomeado e chama o método v2.0
    public async Task<IEnumerable<Unidade>> GetAllUnidadesAsync(Guid tenantId)
    {
        return await _repository.GetAllAsync(tenantId);
    }

    // 4. Renomeado e cria uma Unidade
    public async Task<Unidade> CreateUnidadeAsync(OperacaoDto operacaoDto, Guid userId, Guid tenantId)
    {
        var novaUnidade = new Unidade // <-- MUDANÇA AQUI
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId, 
            Nome = operacaoDto.Nome,
            Descricao = operacaoDto.Descricao,
            Endereco = operacaoDto.Endereco,
            MetaMensal = operacaoDto.MetaMensal,
            DataInicio = DateTime.SpecifyKind(operacaoDto.DataInicio, DateTimeKind.Utc),
            DataFim = operacaoDto.DataFim.HasValue ? DateTime.SpecifyKind(operacaoDto.DataFim.Value, DateTimeKind.Utc) : null,
            IsAtiva = true,
            UserId = userId
        };
        await _repository.AddAsync(novaUnidade); // <-- MUDANÇA AQUI

        var novoVinculo = new UsuarioOperacao
        {
            UserId = userId,
            UnidadeId = novaUnidade.Id, // <-- Corrigido na refatoração anterior
            TenantId = tenantId 
        };
        await _repository.AddUsuarioOperacaoLinkAsync(novoVinculo);

        await _repository.SaveChangesAsync();

        return novaUnidade;
    }

    // 5. Renomeado
    public async Task<Unidade?> GetUnidadeByIdAsync(Guid id, Guid tenantId)
    {
        return await _repository.GetByIdAsync(id, tenantId);
    }

    // 6. Renomeado
    public async Task<bool> UpdateUnidadeAsync(Guid id, UpdateOperacaoDto operacaoDto, Guid tenantId)
    {
        var unidadeExistente = await _repository.GetByIdAsync(id, tenantId); // <-- MUDANÇA AQUI

        if (unidadeExistente == null)
        {
            return false;
        }

        unidadeExistente.Nome = operacaoDto.Nome;
        unidadeExistente.Descricao = operacaoDto.Descricao;
        unidadeExistente.Endereco = operacaoDto.Endereco;
        unidadeExistente.MetaMensal = operacaoDto.MetaMensal;
        unidadeExistente.DataInicio = DateTime.SpecifyKind(operacaoDto.DataInicio, DateTimeKind.Utc);
        unidadeExistente.DataFim = operacaoDto.DataFim.HasValue
            ? DateTime.SpecifyKind(operacaoDto.DataFim.Value, DateTimeKind.Utc)
            : null;

        _repository.Update(unidadeExistente); // <-- MUDANÇA AQUI
        await _repository.SaveChangesAsync();

        return true;
    }

    // 7. Renomeado
    public async Task<bool> DeactivateUnidadeAsync(Guid id, Guid tenantId)
    {
        var unidadeExistente = await _repository.GetByIdAsync(id, tenantId); // <-- MUDANÇA AQUI

        if (unidadeExistente == null)
        {
            return false;
        }

        unidadeExistente.IsAtiva = false;
        await _repository.SaveChangesAsync();
        return true;
    }
    
    // 8. Renomeado
    public async Task<bool> DeleteUnidadeAsync(Guid id, Guid tenantId)
    {
        var unidadeExistente = await _repository.GetByIdAsync(id, tenantId); // <-- MUDANÇA AQUI
        if (unidadeExistente == null) return false;

        _repository.Remove(unidadeExistente); // <-- MUDANÇA AQUI
        await _repository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateProjecaoAsync(Guid id, decimal projecao)
    {
        return await _repository.UpdateProjecaoAsync(id, projecao);
    }
}