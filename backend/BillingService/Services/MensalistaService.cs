using BillingService.DTO;
using BillingService.Models;
using BillingService.Repositories;

namespace BillingService.Services;

public class MensalistaService
{
    private readonly MensalistaRepository _repository;
    private readonly OperacaoRepository _operacaoRepository;

    public MensalistaService(MensalistaRepository repository, OperacaoRepository operacaoRepository)
    {
        _repository = repository;
        _operacaoRepository = operacaoRepository;
    }

    public async Task<IEnumerable<Mensalista>> GetAllMensalistasAsync(Guid operacaoId, Guid userId)
    {
        // Valida se o admin tem acesso a esta operação antes de listar os mensalistas
        var operacao = await _operacaoRepository.GetByIdAndUserIdAsync(operacaoId, userId);
        if (operacao == null)
        {
            // Retorna lista vazia se não houver permissão, para não vazar informação.
            return new List<Mensalista>();
        }
        return await _repository.GetAllByOperacaoIdAsync(operacaoId);
    }

    public async Task<(Mensalista? mensalista, string? errorMessage)> CreateMensalistaAsync(Guid operacaoId, CreateMensalistaDto mensalistaDto, Guid userId)
    {
        var operacao = await _operacaoRepository.GetByIdAndUserIdAsync(operacaoId, userId);
        if (operacao == null)
        {
            return (null, "Operação não encontrada ou não pertence ao usuário.");
        }

        var novoMensalista = new Mensalista
        {
            Id = Guid.NewGuid(),
            Nome = mensalistaDto.Nome,
            CPF = mensalistaDto.CPF,
            ValorMensalidade = mensalistaDto.ValorMensalidade,
            OperacaoId = operacaoId,
            EmpresaId = mensalistaDto.EmpresaId,
            IsAtivo = true
        };

        await _repository.AddAsync(novoMensalista);
        await _repository.SaveChangesAsync();
        return (novoMensalista, null);
    }

    public async Task<(bool success, string? errorMessage)> UpdateMensalistaAsync(Guid operacaoId, Guid mensalistaId, UpdateMensalistaDto mensalistaDto, Guid userId)
    {
        var operacao = await _operacaoRepository.GetByIdAndUserIdAsync(operacaoId, userId);
        if (operacao == null)
        {
            return (false, "Operação não encontrada ou não pertence ao usuário.");
        }

        var mensalistaExistente = await _repository.GetByIdAsync(mensalistaId);
        if (mensalistaExistente == null || mensalistaExistente.OperacaoId != operacaoId)
        {
            return (false, "Mensalista não encontrado nesta operação.");
        }

        mensalistaExistente.Nome = mensalistaDto.Nome;
        mensalistaExistente.CPF = mensalistaDto.CPF;
        mensalistaExistente.ValorMensalidade = mensalistaDto.ValorMensalidade;
        mensalistaExistente.IsAtivo = mensalistaDto.IsAtivo;
        mensalistaExistente.EmpresaId = mensalistaDto.EmpresaId;

        _repository.Update(mensalistaExistente);
        await _repository.SaveChangesAsync();
        return (true, null);
    }

    public async Task<(bool success, string? errorMessage)> DeactivateMensalistaAsync(Guid operacaoId, Guid mensalistaId, Guid userId)
    {
        var operacao = await _operacaoRepository.GetByIdAndUserIdAsync(operacaoId, userId);
        if (operacao == null)
        {
            return (false, "Operação não encontrada ou não pertence ao usuário.");
        }

        var mensalistaExistente = await _repository.GetByIdAsync(mensalistaId);
        if (mensalistaExistente == null || mensalistaExistente.OperacaoId != operacaoId)
        {
            return (false, "Mensalista não encontrado nesta operação.");
        }

        mensalistaExistente.IsAtivo = false;
        _repository.Update(mensalistaExistente);
        await _repository.SaveChangesAsync();
        return (true, null);
    }
}