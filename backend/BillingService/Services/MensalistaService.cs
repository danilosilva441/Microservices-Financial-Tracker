using BillingService.DTO;
using BillingService.Models;
using BillingService.Repositories.Interfaces;
using BillingService.Services.Interfaces; 
using SharedKernel;

namespace BillingService.Services;

public class MensalistaService : IMensalistaService 
{
    private readonly IMensalistaRepository _repository;
    private readonly IUnidadeRepository _unidadeRepository;

    public MensalistaService(IMensalistaRepository repository, IUnidadeRepository unidadeRepository) 
    {
        _repository = repository;
        _unidadeRepository = unidadeRepository;
    }
    
    // 1. MUDANÇA (v2.0): Parâmetro renomeado para 'unidadeId'
    public async Task<IEnumerable<Mensalista>> GetAllMensalistasAsync(Guid unidadeId, Guid tenantId)
    {
        var unidade = await _unidadeRepository.GetByIdAsync(unidadeId, tenantId);
        if (unidade == null)
        {
            return new List<Mensalista>();
        }
        
        // 2. CORREÇÃO (v2.0): Chamando o método v2.0 (corrige o erro CS1061)
        return await _repository.GetAllByUnidadeIdAsync(unidadeId); 
    }
    
    // 3. MUDANÇA (v2.0): Parâmetro renomeado para 'unidadeId'
    public async Task<(Mensalista? mensalista, string? errorMessage)> CreateMensalistaAsync(Guid unidadeId, CreateMensalistaDto mensalistaDto, Guid tenantId)
    {
        var unidade = await _unidadeRepository.GetByIdAsync(unidadeId, tenantId);
        if (unidade == null)
        {
            return (null, ErrorMessages.UnidadeNotFound);
        }

        var novoMensalista = new Mensalista
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId, 
            Nome = mensalistaDto.Nome,
            CPF = mensalistaDto.CPF,
            ValorMensalidade = mensalistaDto.ValorMensalidade,
            UnidadeId = unidadeId, // (Já estava correto)
            IsAtivo = true
        };

        await _repository.AddAsync(novoMensalista);
        await _repository.SaveChangesAsync();
        return (novoMensalista, null);
    }
    
    // 4. MUDANÇA (v2.0): Parâmetro renomeado para 'unidadeId'
    public async Task<(bool success, string? errorMessage)> UpdateMensalistaAsync(Guid unidadeId, Guid mensalistaId, UpdateMensalistaDto mensalistaDto, Guid tenantId)
    {
        var unidade = await _unidadeRepository.GetByIdAsync(unidadeId, tenantId);
        if (unidade == null)
        {
            return (false, "Unidade não encontrada ou não pertence a este Tenant.");
        }

        var mensalistaExistente = await _repository.GetByIdAsync(mensalistaId);
        if (mensalistaExistente == null || mensalistaExistente.UnidadeId != unidadeId)
        {
            return (false, "Mensalista não encontrado nesta operação.");
        }

        mensalistaExistente.Nome = mensalistaDto.Nome;
        mensalistaExistente.CPF = mensalistaDto.CPF;
        mensalistaExistente.ValorMensalidade = mensalistaDto.ValorMensalidade;
        mensalistaExistente.IsAtivo = mensalistaDto.IsAtivo;

        _repository.Update(mensalistaExistente);
        await _repository.SaveChangesAsync();
        return (true, null);
    }
    
    // 5. MUDANÇA (v2.0): Parâmetro renomeado para 'unidadeId'
    public async Task<(bool success, string? errorMessage)> DeactivateMensalistaAsync(Guid unidadeId, Guid mensalistaId, Guid tenantId)
    {
        var unidade = await _unidadeRepository.GetByIdAsync(unidadeId, tenantId);
        if (unidade == null)
        {
            return (false, "Unidade não encontrada ou não pertence a este Tenant.");
        }

        var mensalistaExistente = await _repository.GetByIdAsync(mensalistaId);
        if (mensalistaExistente == null || mensalistaExistente.UnidadeId != unidadeId)
        {
            return (false, "Mensalista não encontrado nesta operação.");
        }

        mensalistaExistente.IsAtivo = false;
        _repository.Update(mensalistaExistente);
        await _repository.SaveChangesAsync();
        return (true, null);
    }
}