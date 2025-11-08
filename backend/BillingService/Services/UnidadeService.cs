// Caminho: backend/BillingService/Services/UnidadeService.cs
using BillingService.DTOs; // DTOs (Agora temos os DTOs v2.0 corretos)
using BillingService.Models;
using BillingService.Repositories.Interfaces;
using BillingService.Services.Interfaces;

namespace BillingService.Services;

public class UnidadeService : IUnidadeService
{
    private readonly IUnidadeRepository _repository;

    public UnidadeService(IUnidadeRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Unidade>> GetAllUnidadesAsync(Guid tenantId)
    {
        return await _repository.GetAllAsync(tenantId);
    }

    // 1. MUDANÇA: Corrigido de OperacaoDto para UnidadeDto
    public async Task<Unidade> CreateUnidadeAsync(UnidadeDto unidadeDto, Guid userId, Guid tenantId)
    {
        var novaUnidade = new Unidade
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,

            // 2. MUDANÇA: Usando o novo DTO
            Nome = unidadeDto.Nome,
            Descricao = unidadeDto.Descricao,
            Endereco = unidadeDto.Endereco,
            MetaMensal = unidadeDto.MetaMensal,
            DataInicio = DateTime.SpecifyKind(unidadeDto.DataInicio, DateTimeKind.Utc),
            DataFim = unidadeDto.DataFim.HasValue ? DateTime.SpecifyKind(unidadeDto.DataFim.Value, DateTimeKind.Utc) : null,
            IsAtiva = true,
            UserId = userId
        };
        await _repository.AddAsync(novaUnidade);

        var novoVinculo = new UsuarioOperacao
        {
            UserId = userId,
            UnidadeId = novaUnidade.Id,
            TenantId = tenantId,
            RoleInOperation = "Gerente"
        };
        await _repository.AddUsuarioOperacaoLinkAsync(novoVinculo);

        await _repository.SaveChangesAsync();

        return novaUnidade;
    }

    public async Task<Unidade?> GetUnidadeByIdAsync(Guid id, Guid tenantId)
    {
        return await _repository.GetByIdAsync(id, tenantId);
    }

    // 3. MUDANÇA: Corrigido de UpdateOperacaoDto para UpdateUnidadeDto
    public async Task<bool> UpdateUnidadeAsync(Guid id, UpdateUnidadeDto unidadeDto, Guid tenantId)
    {
        var unidadeExistente = await _repository.GetByIdAsync(id, tenantId);

        if (unidadeExistente == null)
        {
            return false;
        }

        // 4. MUDANÇA: Usando o novo DTO
        unidadeExistente.Nome = unidadeDto.Nome;
        unidadeExistente.Descricao = unidadeDto.Descricao;
        unidadeExistente.Endereco = unidadeDto.Endereco;
        unidadeExistente.MetaMensal = unidadeDto.MetaMensal;
        unidadeExistente.DataInicio = DateTime.SpecifyKind(unidadeDto.DataInicio, DateTimeKind.Utc);
        unidadeExistente.DataFim = unidadeDto.DataFim.HasValue
            ? DateTime.SpecifyKind(unidadeDto.DataFim.Value, DateTimeKind.Utc)
            : null;

        _repository.Update(unidadeExistente);
        await _repository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeactivateUnidadeAsync(Guid id, Guid tenantId)
    {
        var unidadeExistente = await _repository.GetByIdAsync(id, tenantId);

        if (unidadeExistente == null)
        {
            return false;
        }

        unidadeExistente.IsAtiva = false;
        await _repository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteUnidadeAsync(Guid id, Guid tenantId)
    {
        var unidadeExistente = await _repository.GetByIdAsync(id, tenantId);
        if (unidadeExistente == null) return false;

        _repository.Remove(unidadeExistente);
        await _repository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateProjecaoAsync(Guid id, decimal projecao)
    {
        return await _repository.UpdateProjecaoAsync(id, projecao);
    }
}