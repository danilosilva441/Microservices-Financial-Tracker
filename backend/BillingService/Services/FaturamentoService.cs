using BillingService.DTO;
using BillingService.Models;
using BillingService.Repositories;

namespace BillingService.Services;

public class FaturamentoService
{
    private readonly FaturamentoRepository _repository;

    public FaturamentoService(FaturamentoRepository repository)
    {
        _repository = repository;
    }

    public async Task<(Faturamento? faturamento, string? errorMessage)> AddFaturamentoAsync(Guid operacaoId, FaturamentoDto faturamentoDto, Guid userId)
    {
        if (!await _repository.OperacaoExistsAndBelongsToUserAsync(operacaoId, userId))
        {
            return (null, "Operação não encontrada.");
        }

        var dataFaturamentoUtc = DateTime.SpecifyKind(faturamentoDto.Data, DateTimeKind.Utc);

        if (await _repository.FaturamentoExistsOnDateAsync(operacaoId, dataFaturamentoUtc))
        {
            return (null, "Já existe um faturamento para esta data.");
        }

        var novoFaturamento = new Faturamento
        {
            Id = Guid.NewGuid(),
            Valor = faturamentoDto.Valor,
            Data = dataFaturamentoUtc,
            Moeda = faturamentoDto.Moeda,
            OperacaoId = operacaoId
        };

        await _repository.AddAsync(novoFaturamento);
        await _repository.SaveChangesAsync();

        return (novoFaturamento, null);
    }

    public async Task<(bool success, string? errorMessage)> UpdateFaturamentoAsync(Guid operacaoId, Guid faturamentoId, UpdateFaturamentoDto faturamentoDto, Guid userId)
    {
        if (!await _repository.OperacaoExistsAndBelongsToUserAsync(operacaoId, userId))
        {
            return (false, "Operação não encontrada.");
        }

        var faturamentoExistente = await _repository.GetByIdAsync(faturamentoId);
        if (faturamentoExistente == null || faturamentoExistente.OperacaoId != operacaoId)
        {
            return (false, "Faturamento não encontrado.");
        }

        var dataFaturamentoUtc = DateTime.SpecifyKind(faturamentoDto.Data, DateTimeKind.Utc);

        if (await _repository.FaturamentoExistsOnDateAsync(operacaoId, dataFaturamentoUtc, faturamentoId))
        {
            return (false, "Já existe outro faturamento para esta data.");
        }

        faturamentoExistente.Valor = faturamentoDto.Valor;
        faturamentoExistente.Data = dataFaturamentoUtc;
        faturamentoExistente.Moeda = faturamentoDto.Moeda;

        _repository.Update(faturamentoExistente);
        await _repository.SaveChangesAsync();
        return (true, null);
    }

    public async Task<(bool success, string? errorMessage)> DeleteFaturamentoAsync(Guid operacaoId, Guid faturamentoId, Guid userId)
    {
        if (!await _repository.OperacaoExistsAndBelongsToUserAsync(operacaoId, userId))
        {
            return (false, "Operação não encontrada.");
        }

        var faturamentoExistente = await _repository.GetByIdAsync(faturamentoId);
        if (faturamentoExistente == null || faturamentoExistente.OperacaoId != operacaoId)
        {
            return (false, "Faturamento não encontrado.");
        }

        _repository.Remove(faturamentoExistente);
        await _repository.SaveChangesAsync();
        return (true, null);
    }
}