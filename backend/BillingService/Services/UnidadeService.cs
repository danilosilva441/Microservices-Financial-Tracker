// Caminho: backend/BillingService/Services/UnidadeService.cs
using BillingService.DTOs; // 1. IMPORTANTE: Usando DTOs (plural)
using BillingService.Models;
using BillingService.Repositories.Interfaces; 
using BillingService.Services.Interfaces; 

namespace BillingService.Services;

// 2. Implementa a interface v2.0
public class UnidadeService : IUnidadeService 
{
    private readonly IUnidadeRepository _repository;

    public UnidadeService(IUnidadeRepository repository)
    {
        _repository = repository;
    }

    // 3. NOVO (v2.0): Implementa o método para Gerentes
    public async Task<IEnumerable<Unidade>> GetAllUnidadesByTenantAsync(Guid tenantId)
    {
        return await _repository.GetAllAsync(tenantId);
    }

    // 4. NOVO (v2.0): Implementa o método para Admins/Sistema
    public async Task<IEnumerable<Unidade>> GetAllUnidadesAdminAsync()
    {
        return await _repository.GetAllAdminAsync();
    }

    // 5. CORRIGIDO (v2.0): Usa o DTO "UnidadeDto"
    public async Task<Unidade> CreateUnidadeAsync(UnidadeDto unidadeDto, Guid userId, Guid tenantId)
    {
        var novaUnidade = new Unidade 
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId, 
            // 6. CORRIGIDO (v2.0): Mapeia do DTO "UnidadeDto"
            Nome = unidadeDto.Nome,
            Descricao = unidadeDto.Descricao,
            Endereco = unidadeDto.Endereco,
            MetaMensal = unidadeDto.MetaMensal,
            DataInicio = DateTime.SpecifyKind(unidadeDto.DataInicio, DateTimeKind.Utc),
            DataFim = unidadeDto.DataFim.HasValue ? DateTime.SpecifyKind(unidadeDto.DataFim.Value, DateTimeKind.Utc) : null,
            IsAtiva = true,
            UserId = userId
            // TODO: Adicionar os novos campos de customização (AceitaPix, etc.) do modelo Unidade.cs
        };
        await _repository.AddAsync(novaUnidade);

        var novoVinculo = new UsuarioOperacao
        {
            UserId = userId,
            UnidadeId = novaUnidade.Id, 
            TenantId = tenantId, 
            RoleInOperation = "Gerente" // O criador é o Gerente
        };
        await _repository.AddUsuarioOperacaoLinkAsync(novoVinculo);

        await _repository.SaveChangesAsync();

        return novaUnidade;
    }
    
    public async Task<Unidade?> GetUnidadeByIdAsync(Guid id, Guid tenantId)
    {
        return await _repository.GetByIdAsync(id, tenantId);
    }

    // 7. CORRIGIDO (v2.0): Usa o DTO "UpdateUnidadeDto"
    public async Task<bool> UpdateUnidadeAsync(Guid id, UpdateUnidadeDto unidadeDto, Guid tenantId)
    {
        var unidadeExistente = await _repository.GetByIdAsync(id, tenantId);

        if (unidadeExistente == null)
        {
            return false;
        }

        // 8. CORRIGIDO (v2.0): Mapeia do DTO "UpdateUnidadeDto"
        unidadeExistente.Nome = unidadeDto.Nome;
        unidadeExistente.Descricao = unidadeDto.Descricao;
        unidadeExistente.Endereco = unidadeDto.Endereco;
        unidadeExistente.MetaMensal = unidadeDto.MetaMensal;
        unidadeExistente.DataInicio = DateTime.SpecifyKind(unidadeDto.DataInicio, DateTimeKind.Utc);
        unidadeExistente.DataFim = unidadeDto.DataFim.HasValue
            ? DateTime.SpecifyKind(unidadeDto.DataFim.Value, DateTimeKind.Utc)
            : null;
        
        // TODO: Mapear os campos de customização (AceitaPix, etc.) se eles estiverem no DTO

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