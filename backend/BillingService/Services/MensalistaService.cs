// Caminho: backend/BillingService/Services/MensalistaService.cs
using BillingService.DTOs;
using BillingService.Models;
using BillingService.Repositories.Interfaces; // IMPORTANTE
using BillingService.Services.Interfaces; // IMPORTANTE

namespace BillingService.Services;

// 1. MUDANÇA: Herda da interface v2.0
public class MensalistaService : IMensalistaService
{
    // 2. MUDANÇA: Injeta INTERFACES v2.0
    private readonly IMensalistaRepository _repository;
    private readonly IUnidadeRepository _unidadeRepository;

    public MensalistaService(IMensalistaRepository repository, IUnidadeRepository unidadeRepository)
    {
        _repository = repository;
        _unidadeRepository = unidadeRepository;
    }

    // 3. MUDANÇA: Assinatura v2.0 (unidadeId, tenantId)
    public async Task<IEnumerable<Mensalista>> GetAllMensalistasAsync(Guid unidadeId, Guid tenantId)
    {
        // 4. MUDANÇA: Chama o método v2.0 do repositório
        var unidade = await _unidadeRepository.GetByIdAsync(unidadeId, tenantId);
        if (unidade == null)
        {
            return new List<Mensalista>();
        }
        
        // 5. MUDANÇA: Chama o método v2.0 do repositório
        // (Nota: Isso vai quebrar o MensalistaRepository, que corrigiremos a seguir)
        return await _repository.GetAllByUnidadeIdAsync(unidadeId);
    }

    // 6. MUDANÇA: Assinatura v2.0
    public async Task<(Mensalista? mensalista, string? errorMessage)> CreateMensalistaAsync(Guid unidadeId, CreateMensalistaDto mensalistaDto, Guid tenantId)
    {
        // 7. MUDANÇA: Chama o método v2.0
        var unidade = await _unidadeRepository.GetByIdAsync(unidadeId, tenantId);
        if (unidade == null)
        {
            return (null, "Unidade não encontrada ou não pertence a este Tenant.");
        }

        var novoMensalista = new Mensalista
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId, // <-- v2.0
            Nome = mensalistaDto.Nome,
            CPF = mensalistaDto.CPF,
            ValorMensalidade = mensalistaDto.ValorMensalidade,
            UnidadeId = unidadeId, // <-- v2.0 (substitui OperacaoId)
            // EmpresaId = ... // <-- v1.0 (REMOVIDO)
            IsAtivo = true
        };

        await _repository.AddAsync(novoMensalista);
        await _repository.SaveChangesAsync();
        return (novoMensalista, null);
    }

    // 8. MUDANÇA: Assinatura v2.0
    public async Task<(bool success, string? errorMessage)> UpdateMensalistaAsync(Guid unidadeId, Guid mensalistaId, UpdateMensalistaDto mensalistaDto, Guid tenantId)
    {
        // 9. MUDANÇA: Chama o método v2.0
        var unidade = await _unidadeRepository.GetByIdAsync(unidadeId, tenantId);
        if (unidade == null)
        {
            return (false, "Unidade não encontrada ou não pertence a este Tenant.");
        }

        var mensalistaExistente = await _repository.GetByIdAsync(mensalistaId);
        // 10. MUDANÇA: v2.0
        if (mensalistaExistente == null || mensalistaExistente.UnidadeId != unidadeId)
        {
            return (false, "Mensalista não encontrado nesta operação.");
        }

        mensalistaExistente.Nome = mensalistaDto.Nome;
        mensalistaExistente.CPF = mensalistaDto.CPF;
        mensalistaExistente.ValorMensalidade = mensalistaDto.ValorMensalidade;
        mensalistaExistente.IsAtivo = mensalistaDto.IsAtivo;
        // EmpresaId = ... // <-- v1.0 (REMOVIDO)

        _repository.Update(mensalistaExistente);
        await _repository.SaveChangesAsync();
        return (true, null);
    }

    // 11. MUDANÇA: Assinatura v2.0
    public async Task<(bool success, string? errorMessage)> DeactivateMensalistaAsync(Guid unidadeId, Guid mensalistaId, Guid tenantId)
    {
        // 12. MUDANÇA: Chama o método v2.0
        var unidade = await _unidadeRepository.GetByIdAsync(unidadeId, tenantId);
        if (unidade == null)
        {
            return (false, "Unidade não encontrada ou não pertence a este Tenant.");
        }

        var mensalistaExistente = await _repository.GetByIdAsync(mensalistaId);
        // 13. MUDANÇA: v2.0
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