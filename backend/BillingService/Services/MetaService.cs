using BillingService.DTOs;
using BillingService.Models;
using BillingService.Repositories.Interfaces;
using BillingService.Services.Interfaces;
using Microsoft.Extensions.Logging;
using SharedKernel;
using SharedKernel.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BillingService.Services.Exceptions;

namespace BillingService.Services;

public class MetaService : IMetaService
{
    private readonly IMetaRepository _repository;
    private readonly IUnidadeRepository _unidadeRepository;
    private readonly ILogger<MetaService> _logger;

    public MetaService(
        IMetaRepository repository, 
        IUnidadeRepository unidadeRepository,
        ILogger<MetaService> logger)
    {
        _repository = repository;
        _unidadeRepository = unidadeRepository;
        _logger = logger;
    }

    public async Task<Meta?> GetMetaAsync(Guid unidadeId, int mes, int ano, Guid tenantId)
    {
        try
        {
            _logger.LogDebug("Buscando meta para unidade {UnidadeId}, período {Mes}/{Ano}", 
                unidadeId, mes, ano);

            ValidatePeriodo(mes, ano);

            return await _repository.GetByUnidadeAndPeriodAsync(unidadeId, mes, ano, tenantId);
        }
        catch (BaseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar meta para unidade {UnidadeId}", unidadeId);
            throw new MetaServiceException("Erro ao buscar meta", ex);
        }
    }
    
    public async Task<IEnumerable<Meta>> GetMetasAsync(Guid unidadeId, Guid tenantId)
    {
        try
        {
            _logger.LogDebug("Buscando todas as metas para unidade {UnidadeId}", unidadeId);
            
            await ValidateUnidadeExistsAsync(unidadeId, tenantId);
            
            return await _repository.GetAllByUnidadeAsync(unidadeId, tenantId);
        }
        catch (BaseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar metas para unidade {UnidadeId}", unidadeId);
            throw new MetaServiceException("Erro ao buscar metas", ex);
        }
    }

    public async Task<(Meta? meta, string? errorMessage)> SetMetaAsync(
        Guid unidadeId, MetaDto metaDto, Guid tenantId)
    {
        try
        {
            _logger.LogInformation(
                "Definindo meta para unidade {UnidadeId}: {ValorAlvo} em {Mes}/{Ano}", 
                unidadeId, metaDto.ValorAlvo, metaDto.Mes, metaDto.Ano);

            // Validações
            ValidateMetaDto(metaDto);
            await ValidateUnidadeExistsAsync(unidadeId, tenantId);

            var metaExistente = await _repository.GetByUnidadeAndPeriodAsync(
                unidadeId, metaDto.Mes, metaDto.Ano, tenantId);

            if (metaExistente != null)
            {
                return await UpdateMetaExistenteAsync(metaExistente, metaDto.ValorAlvo);
            }
            else
            {
                return await CreateNovaMetaAsync(unidadeId, metaDto, tenantId);
            }
        }
        catch (BaseException ex)
        {
            _logger.LogWarning(ex, "Validação falhou ao definir meta para unidade {UnidadeId}", unidadeId);
            return (null, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao definir meta para unidade {UnidadeId}", unidadeId);
            return (null, "Erro interno ao processar a meta");
        }
    }

    #region Private Methods

    private async Task ValidateUnidadeExistsAsync(Guid unidadeId, Guid tenantId)
    {
        var unidade = await _unidadeRepository.GetByIdAsync(unidadeId, tenantId);
        if (unidade == null)
        {
            // ✅ USANDO EntityNotFoundException EM VEZ DE UnidadeNotFoundException
            throw new NotFoundException("Unidade", unidadeId);
        }
    }

    private static void ValidateMetaDto(MetaDto metaDto)
    {
        if (metaDto == null)
            throw new ArgumentNullException(nameof(metaDto));

        ValidatePeriodo(metaDto.Mes, metaDto.Ano);
        ValidateValorAlvo(metaDto.ValorAlvo);
    }

    private static void ValidatePeriodo(int mes, int ano)
    {
        if (mes < 1 || mes > 12)
            throw new BusinessException("MES_INVALIDO", ErrorCodes.InvalidNumberRange, "Mês deve estar entre 1 e 12");

        var anoAtual = DateTime.Now.Year;
        if (ano < anoAtual - 1 || ano > anoAtual + 5)
            throw new BusinessException("ANO_INVALIDO", ErrorCodes.InvalidNumberRange, $"Ano deve estar entre {anoAtual -1} e {anoAtual +5}");
    }

    private static void ValidateValorAlvo(decimal valorAlvo)
    {
        if (valorAlvo <= 0)
            throw new BusinessException("VALOR_ALVO_INVALIDO", ErrorCodes.RequiredField, "Valor alvo deve ser maior que zero");

        if (valorAlvo > 10_000_000) // 10 milhões
            throw new BusinessException("VALOR_ALVO_MUITO_ALTO", errorCode: ErrorCodes.InvalidNumberRange, "Valor alvo muito alto");
    }

    private async Task<(Meta? meta, string? errorMessage)> UpdateMetaExistenteAsync(
        Meta metaExistente, decimal novoValorAlvo)
    {
        if (metaExistente.ValorAlvo == novoValorAlvo)
        {
            _logger.LogInformation(
                "Meta {MetaId} já possui o valor alvo {ValorAlvo}", 
                metaExistente.Id, novoValorAlvo);
            return (metaExistente, null);
        }

        var valorAnterior = metaExistente.ValorAlvo;
        metaExistente.ValorAlvo = novoValorAlvo;
        
        _repository.Update(metaExistente);
        await _repository.SaveChangesAsync();

        _logger.LogInformation(
            "Meta {MetaId} atualizada: {ValorAnterior} → {NovoValor}", 
            metaExistente.Id, valorAnterior, novoValorAlvo);

        return (metaExistente, null);
    }

    private async Task<(Meta? meta, string? errorMessage)> CreateNovaMetaAsync(
        Guid unidadeId, MetaDto metaDto, Guid tenantId)
    {
        var novaMeta = new Meta
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            UnidadeId = unidadeId,
            Mes = metaDto.Mes,
            Ano = metaDto.Ano,
            ValorAlvo = metaDto.ValorAlvo,
        };

        await _repository.AddAsync(novaMeta);
        await _repository.SaveChangesAsync();

        _logger.LogInformation(
            "Nova meta criada: {MetaId} para unidade {UnidadeId}", 
            novaMeta.Id, unidadeId);

        return (novaMeta, null);
    }

    #endregion
}