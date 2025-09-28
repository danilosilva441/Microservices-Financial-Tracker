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
        // CORREÇÃO: Usa apenas o método do repositório para a validação.
        // 1. Validação de Permissão
        if (!await _repository.UserHasAccessToOperacaoAsync(operacaoId, userId))
        {
            return (null, "Operação não encontrada ou o usuário não tem permissão para acessá-la.");
        }

        // Em vez de converter, nós "carimbamos" a data como sendo UTC.
        // Isso preserva o dia que o usuário selecionou no frontend.
        var dataFaturamentoUtc = DateTime.SpecifyKind(faturamentoDto.Data, DateTimeKind.Utc);

        // 2. REGRA: NÃO PERMITIR DATAS FUTURAS
        /*
        if (dataFaturamentoUtc.Date > DateTime.UtcNow.Date)
        {
            return (null, "Não é permitido registrar um faturamento para uma data futura.");
        }
        */

        // 3. REGRA: JANELA DE LANÇAMENTO (D+1)
        // Exemplo: Se hoje é dia 28, o faturamento só pode ser do dia 27.
        if (DateTime.UtcNow.Date != dataFaturamentoUtc.Date.AddDays(1))
        {
            return (null, $"O faturamento só pode ser registrado no dia seguinte à data da ocorrência. Hoje é {DateTime.UtcNow.ToShortDateString()}, portanto, só são aceitos faturamentos do dia {DateTime.UtcNow.Date.AddDays(-1).ToShortDateString()}.");
        }

        // 4. REGRA: NÃO PERMITIR DUPLICIDADE
        if (await _repository.FaturamentoExistsOnDateAsync(operacaoId, dataFaturamentoUtc))
        {
            return (null, "Já existe um faturamento registrado para esta data.");
        }

        // Cria o novo faturamento se todas as regras passarem
        var novoFaturamento = new Faturamento
        {
            Id = Guid.NewGuid(),
            Valor = faturamentoDto.Valor,
            Data = dataFaturamentoUtc,
            Moeda = faturamentoDto.Moeda,
            Origem = faturamentoDto.Origem,
            IsAtivo = true, // Garante que o novo faturamento seja criado como ativo
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
        // (A regra D+1 pode ou não se aplicar à edição, vamos deixá-la de fora por enquanto para mais flexibilidade)

        // 4. Validação de duplicidade (verifica se já existe OUTRO faturamento na nova data)
        if (await _repository.FaturamentoExistsOnDateAsync(operacaoId, dataFaturamentoUtc, faturamentoId))
        {
            return (false, "Já existe outro faturamento registrado para esta nova data.");
        }

        // 5. Atualiza os dados
        faturamentoExistente.Valor = faturamentoDto.Valor;
        faturamentoExistente.Data = dataFaturamentoUtc;
        faturamentoExistente.Moeda = faturamentoDto.Moeda;

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