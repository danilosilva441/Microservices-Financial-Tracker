// Caminho: backend/BillingService/Services/MetaService.cs
using BillingService.DTOs;
using BillingService.Models;
using BillingService.Repositories.Interfaces; // 1. IMPORTANTE: Usando Interfaces
using BillingService.Services.Interfaces; // 1. IMPORTANTE: Usando Interfaces

namespace BillingService.Services;

public class MetaService : IMetaService // 2. Herda da interface
{
    // 3. MUDANÇA: Injeta INTERFACES v2.0
    private readonly IMetaRepository _repository;
    private readonly IUnidadeRepository _unidadeRepository;

    public MetaService(IMetaRepository repository, IUnidadeRepository unidadeRepository)
    {
        _repository = repository;
        _unidadeRepository = unidadeRepository;
    }

    // 4. MUDANÇA: Assinatura v2.0
    public async Task<Meta?> GetMetaAsync(Guid unidadeId, int mes, int ano, Guid tenantId)
    {
        return await _repository.GetByUnidadeAndPeriodAsync(unidadeId, mes, ano, tenantId);
    }
    
    // 5. NOVO (v2.0): Lista todas as metas
    public async Task<IEnumerable<Meta>> GetMetasAsync(Guid unidadeId, Guid tenantId)
    {
        return await _repository.GetAllByUnidadeAsync(unidadeId, tenantId);
    }

    // 6. MUDANÇA: Assinatura v2.0
    public async Task<(Meta? meta, string? errorMessage)> SetMetaAsync(Guid unidadeId, MetaDto metaDto, Guid tenantId)
    {
        // 7. NOVO (v2.0): Validação de Unidade
        var unidade = await _unidadeRepository.GetByIdAsync(unidadeId, tenantId);
        if (unidade == null)
        {
            return (null, "Unidade não encontrada ou não pertence a este Tenant.");
        }

        // 8. MUDANÇA: Lógica v2.0
        var metaExistente = await _repository.GetByUnidadeAndPeriodAsync(unidadeId, metaDto.Mes, metaDto.Ano, tenantId);

        if (metaExistente != null)
        {
            metaExistente.ValorAlvo = metaDto.ValorAlvo;
            _repository.Update(metaExistente);
            await _repository.SaveChangesAsync();
            return (metaExistente, null);
        }
        else
        {
            // 9. MUDANÇA: Cria o modelo v2.0
            var novaMeta = new Meta
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId, // <-- v2.0
                UnidadeId = unidadeId, // <-- v2.0
                Mes = metaDto.Mes,
                Ano = metaDto.Ano,
                ValorAlvo = metaDto.ValorAlvo
                // UserId = userId // <-- v1.0 REMOVIDO
            };
            await _repository.AddAsync(novaMeta);
            await _repository.SaveChangesAsync();
            return (novaMeta, null);
        }
    }
}