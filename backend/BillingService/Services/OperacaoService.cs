using BillingService.DTO;
using BillingService.Models;
using BillingService.Repositories;

namespace BillingService.Services;

public class OperacaoService
{
    private readonly OperacaoRepository _repository;

    public OperacaoService(OperacaoRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Operacao>> GetOperacoesByUserAsync(Guid userId, int? ano, int? mes, bool? isAtiva)
    {
        return await _repository.GetByUserIdAsync(userId, ano, mes, isAtiva);
    }

    public async Task<Operacao> CreateOperacaoAsync(OperacaoDto operacaoDto, Guid userId)
    {
        var novaOperacao = new Operacao
        {
            Id = Guid.NewGuid(),
            Nome = operacaoDto.Nome,
            Descricao = operacaoDto.Descricao,
            Endereco = operacaoDto.Endereco,
            Moeda = operacaoDto.Moeda,
            MetaMensal = operacaoDto.MetaMensal,
            DataInicio = DateTime.SpecifyKind(operacaoDto.DataInicio, DateTimeKind.Utc),
            DataFim = operacaoDto.DataFim.HasValue ? DateTime.SpecifyKind(operacaoDto.DataFim.Value, DateTimeKind.Utc) : null,
            IsAtiva = true,
            UserId = userId
        };

        await _repository.AddAsync(novaOperacao);
        await _repository.SaveChangesAsync();

        return novaOperacao;
    }
    public async Task<Operacao?> GetOperacaoByIdAsync(Guid id, Guid userId)
    {
        return await _repository.GetByIdAndUserIdAsync(id, userId);
    }

    public async Task<bool> UpdateOperacaoAsync(Guid id, UpdateOperacaoDto operacaoDto, Guid userId)
    {
        var operacaoExistente = await _repository.GetByIdAndUserIdAsync(id, userId);

        if (operacaoExistente == null)
        {
            return false; // Indica que a operação não foi encontrada
        }

        // Mapeia os dados do DTO para a entidade existente
        operacaoExistente.Nome = operacaoDto.Nome;
        operacaoExistente.Descricao = operacaoDto.Descricao;
        operacaoExistente.Endereco = operacaoDto.Endereco;
        operacaoExistente.Moeda = operacaoDto.Moeda;
        operacaoExistente.MetaMensal = operacaoDto.MetaMensal;
        operacaoExistente.DataInicio = DateTime.SpecifyKind(operacaoDto.DataInicio, DateTimeKind.Utc);
        operacaoExistente.DataFim = operacaoDto.DataFim.HasValue
            ? DateTime.SpecifyKind(operacaoDto.DataFim.Value, DateTimeKind.Utc)
            : null;

        _repository.Update(operacaoExistente);
        await _repository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeactivateOperacaoAsync(Guid id, Guid userId)
    {
        var operacaoExistente = await _repository.GetByIdAndUserIdAsync(id, userId);

        if (operacaoExistente == null)
        {
            return false;
        }

        operacaoExistente.IsAtiva = false;
        await _repository.SaveChangesAsync();
        return true;
    }
}
