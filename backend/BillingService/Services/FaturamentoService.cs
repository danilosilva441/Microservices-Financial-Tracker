using BillingService.DTO;
using BillingService.Models;
using BillingService.Repositories;
using BillingService.Data;
using Microsoft.EntityFrameworkCore;

namespace BillingService.Services;

public class FaturamentoService
{
    private readonly FaturamentoRepository _repository;
    private readonly BillingDbContext _context;


    public FaturamentoService(FaturamentoRepository repository, BillingDbContext context)
    {
        _repository = repository;
        _context = context; // Atribui o valor recebido ao campo da classe
    }

    public async Task<(Faturamento? faturamento, string? errorMessage)> AddFaturamentoAsync(Guid operacaoId, FaturamentoDto faturamentoDto, Guid userId)
    {
        // Agora o .AnyAsync() será encontrado por causa do 'using Microsoft.EntityFrameworkCore;'
        var vinculoExiste = await _context.UsuarioOperacoes.AnyAsync(uo => uo.OperacaoId == operacaoId && uo.UserId == userId);

        if (!vinculoExiste)
        {
            return (null, "A operação não foi encontrada ou o usuário não tem permissão para acessá-la.");
        }

        var dataFaturamentoUtc = DateTime.SpecifyKind(faturamentoDto.Data, DateTimeKind.Utc);

        if (await _repository.FaturamentoExistsOnDateAsync(operacaoId, dataFaturamentoUtc))
        {
            return (null, "Já existe um faturamento registrado para esta data.");
        }

        var novoFaturamento = new Faturamento
        {
            Id = Guid.NewGuid(),
            Valor = faturamentoDto.Valor,
            Data = dataFaturamentoUtc,
            Moeda = faturamentoDto.Moeda,
            Origem = faturamentoDto.Origem,
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