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
        // 1. Validação de Permissão
        if (!await _repository.UserHasAccessToOperacaoAsync(operacaoId, userId))
        {
            return (null, "Operação não encontrada ou o usuário não tem permissão para acessá-la.");
        }

        // Converte a data para UTC preservando o dia
        var dataFaturamentoUtc = DateTime.SpecifyKind(faturamentoDto.Data, DateTimeKind.Utc);

        // 2. REGRA: NÃO PERMITIR DATAS FUTURAS
        if (dataFaturamentoUtc.Date > DateTime.UtcNow.Date)
        {
            return (null, "Não é permitido registrar um faturamento para uma data futura.");
        }

        // 3. REGRA FLEXÍVEL: Permite faturamentos de até 30 dias no passado
        var diasDiferenca = (DateTime.UtcNow.Date - dataFaturamentoUtc.Date).Days;
        if (diasDiferenca > 30)
        {
            return (null, $"Não é permitido registrar faturamentos com mais de 30 dias de diferença. Data máxima permitida: {DateTime.UtcNow.Date.AddDays(-30).ToShortDateString()}.");
        }

        // 4. REGRA: NÃO PERMITIR DUPLICIDADE
        if (await _repository.FaturamentoExistsOnDateAsync(operacaoId, dataFaturamentoUtc))
        {
            return (null, "Já existe um faturamento registrado para esta data.");
        }

        // Cria o novo faturamento
        var novoFaturamento = new Faturamento
        {
            Id = Guid.NewGuid(),
            Valor = faturamentoDto.Valor,
            Data = dataFaturamentoUtc,
            Origem = faturamentoDto.Origem,
            IsAtivo = true,
            OperacaoId = operacaoId
        };

        await _repository.AddAsync(novoFaturamento);
        await _repository.SaveChangesAsync();

        return (novoFaturamento, null);
    }

    public async Task<(bool success, string? errorMessage)> UpdateFaturamentoAsync(Guid operacaoId, Guid faturamentoId, UpdateFaturamentoDto faturamentoDto, Guid userId)
    {
        // 1. Verifica se o usuário tem acesso à operação pai
        if (!await _repository.UserHasAccessToOperacaoAsync(operacaoId, userId))
        {
            return (false, "Operação não encontrada ou usuário sem permissão.");
        }

        // 2. Busca o faturamento específico que será atualizado
        var faturamentoExistente = await _repository.GetByIdAsync(faturamentoId);
        if (faturamentoExistente == null || faturamentoExistente.OperacaoId != operacaoId)
        {
            return (false, "Faturamento não encontrado nesta operação.");
        }

        // 3. Validação de data (igual à da criação)
        var dataFaturamentoUtc = DateTime.SpecifyKind(faturamentoDto.Data, DateTimeKind.Utc);
        if (dataFaturamentoUtc.Date > DateTime.UtcNow.Date)
        {
            return (false, "Não é permitido atualizar um faturamento para uma data futura.");
        }

        // 4. Validação flexível para atualização (até 30 dias no passado)
        var diasDiferenca = (DateTime.UtcNow.Date - dataFaturamentoUtc.Date).Days;
        if (diasDiferenca > 30)
        {
            return (false, $"Não é permitido atualizar faturamentos para datas com mais de 30 dias de diferença. Data máxima permitida: {DateTime.UtcNow.Date.AddDays(-30).ToShortDateString()}.");
        }

        // 5. Validação de duplicidade (verifica se já existe OUTRO faturamento na nova data)
        if (await _repository.FaturamentoExistsOnDateAsync(operacaoId, dataFaturamentoUtc, faturamentoId))
        {
            return (false, "Já existe outro faturamento registrado para esta nova data.");
        }

        // 6. Atualiza os dados
        faturamentoExistente.Valor = faturamentoDto.Valor;
        faturamentoExistente.Data = dataFaturamentoUtc;

        _repository.Update(faturamentoExistente);
        await _repository.SaveChangesAsync();

        return (true, null);
    }

    public async Task<(bool success, string? errorMessage)> DeleteFaturamentoAsync(Guid operacaoId, Guid faturamentoId, Guid userId)
    {
        // 1. Verifica se o usuário tem acesso à operação pai
        if (!await _repository.UserHasAccessToOperacaoAsync(operacaoId, userId))
        {
            return (false, "Operação não encontrada ou usuário sem permissão.");
        }

        // 2. Busca o faturamento específico que será deletado
        var faturamentoExistente = await _repository.GetByIdAsync(faturamentoId);
        if (faturamentoExistente == null || faturamentoExistente.OperacaoId != operacaoId)
        {
            return (false, "Faturamento não encontrado nesta operação.");
        }

        // 3. Remove o faturamento
        _repository.Remove(faturamentoExistente);
        await _repository.SaveChangesAsync();

        return (true, null);
    }

    public async Task<(bool success, string? errorMessage)> DeactivateFaturamentoAsync(Guid operacaoId, Guid faturamentoId, Guid userId)
    {
        // 1. Verifica se o usuário tem acesso à operação pai
        if (!await _repository.UserHasAccessToOperacaoAsync(operacaoId, userId))
        {
            return (false, "Operação não encontrada ou usuário sem permissão.");
        }

        // 2. Busca o faturamento específico
        var faturamentoExistente = await _repository.GetByIdAsync(faturamentoId);
        if (faturamentoExistente == null || faturamentoExistente.OperacaoId != operacaoId)
        {
            return (false, "Faturamento não encontrado nesta operação.");
        }

        // 3. Altera o status e salva
        faturamentoExistente.IsAtivo = false;
        _repository.Update(faturamentoExistente);
        await _repository.SaveChangesAsync();

        return (true, null);
    }
}