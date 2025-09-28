using BillingService.DTO;
using BillingService.Models;
using BillingService.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BillingService.Services;

public class FaturamentoService
{
    private readonly FaturamentoRepository _repository;

    // CORREÇÃO: Removemos a injeção do DbContext daqui. O serviço não precisa conhecê-lo.
    public FaturamentoService(FaturamentoRepository repository)
    {
        _repository = repository;
    }

    public async Task<(Faturamento? faturamento, string? errorMessage)> AddFaturamentoAsync(Guid operacaoId, FaturamentoDto faturamentoDto, Guid userId)
    {
        // CORREÇÃO: Usamos apenas o método do repositório para a validação.
        if (!await _repository.UserHasAccessToOperacaoAsync(operacaoId, userId))
        {
            return (null, "Operação não encontrada ou o usuário não tem permissão para acessá-la.");
        }

        var dataFaturamentoUtc = DateTime.SpecifyKind(faturamentoDto.Data, DateTimeKind.Utc);

        // ... (resto da lógica de validação de datas, duplicidade, etc. continua igual)

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
        // CORREÇÃO: Usamos apenas o método do repositório para a validação.
        if (!await _repository.UserHasAccessToOperacaoAsync(operacaoId, userId))
        {
            return (false, "Operação não encontrada ou usuário sem permissão.");
        }

        var faturamentoExistente = await _repository.GetByIdAsync(faturamentoId);
        // ... (resto da lógica de atualização)
        return (true, null);
    }

    public async Task<(bool success, string? errorMessage)> DeleteFaturamentoAsync(Guid operacaoId, Guid faturamentoId, Guid userId)
    {
        // CORREÇÃO: Usamos apenas o método do repositório para a validação.
        if (!await _repository.UserHasAccessToOperacaoAsync(operacaoId, userId))
        {
            return (false, "Operação não encontrada ou usuário sem permissão.");
        }

        var faturamentoExistente = await _repository.GetByIdAsync(faturamentoId);
        // ... (resto da lógica de deleção)
        return (true, null);
    }
}