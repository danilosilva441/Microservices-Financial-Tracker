using System.Text.Json;
using BillingService.DTOs;
using BillingService.Models;
using BillingService.Repositories.Interfaces;
using BillingService.Services.Interfaces;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace BillingService.Services;

public class SolicitacaoService : ISolicitacaoService
{
    private readonly ISolicitacaoRepository _solicitacaoRepository;
    private readonly IFaturamentoParcialRepository _faturamentoRepository;
    private readonly IUnidadeRepository _unidadeRepository;
    private readonly ILogger<SolicitacaoService> _logger;

    // Constantes para melhor manutenção
    private const string STATUS_PENDENTE = "PENDENTE";
    private const string STATUS_APROVADA = "APROVADA";
    private const string STATUS_REJEITADA = "REJEITADA";
    private const string STATUS_CANCELADA = "CANCELADA";
    private const string TIPO_REMOCAO = "REMOCAO";
    private const string TIPO_ALTERACAO = "ALTERACAO";
    private const string TIPO_ADICAO = "ADICAO";

    public SolicitacaoService(
        ISolicitacaoRepository solicitacaoRepository,
        IFaturamentoParcialRepository faturamentoRepository,
        IUnidadeRepository unidadeRepository,
        ILogger<SolicitacaoService> logger)
    {
        _solicitacaoRepository = solicitacaoRepository ?? throw new ArgumentNullException(nameof(solicitacaoRepository));
        _faturamentoRepository = faturamentoRepository ?? throw new ArgumentNullException(nameof(faturamentoRepository));
        _unidadeRepository = unidadeRepository ?? throw new ArgumentNullException(nameof(unidadeRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<SolicitacaoAjuste> CriarSolicitacaoAsync(
        SolicitacaoAjuste solicitacao,
        Guid solicitanteId)
    {
        try
        {
            _logger.LogInformation("Iniciando criação de solicitação de ajuste. Solicitante: {SolicitanteId}", solicitanteId);

            if (solicitacao == null)
            {
                _logger.LogError("Tentativa de criar solicitação com objeto nulo");
                throw new ArgumentNullException(nameof(solicitacao));
            }

            if (solicitanteId == Guid.Empty)
            {
                _logger.LogError("Tentativa de criar solicitação com solicitanteId vazio");
                throw new ArgumentException("ID do solicitante inválido", nameof(solicitanteId));
            }

            // Validação básica do tipo de solicitação
            if (!ValidarTipoSolicitacao(solicitacao.Tipo))
            {
                _logger.LogWarning("Tipo de solicitação inválido: {Tipo}", solicitacao.Tipo);
                throw new ArgumentException($"Tipo de solicitação inválido: {solicitacao.Tipo}");
            }

            solicitacao.SolicitanteId = solicitanteId;
            solicitacao.Status = STATUS_PENDENTE;
            solicitacao.DataSolicitacao = DateTime.UtcNow;

            _logger.LogDebug("Adicionando solicitação ao repositório. Tipo: {Tipo}, Motivo: {Motivo}", 
                solicitacao.Tipo, solicitacao.Motivo);

            await _solicitacaoRepository.AddAsync(solicitacao);
            await _solicitacaoRepository.SaveChangesAsync();

            _logger.LogInformation("Solicitação criada com sucesso. ID: {SolicitacaoId}", solicitacao.Id);

            return solicitacao;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar solicitação de ajuste. Solicitante: {SolicitanteId}", solicitanteId);
            throw;
        }
    }

    public async Task<IEnumerable<SolicitacaoAjusteDto>> GetSolicitacoesAsync()
    {
        try
        {
            _logger.LogDebug("Buscando todas as solicitações de ajuste");

            var solicitacoes = await _solicitacaoRepository.GetAllComDetalhesAsync();
            var resultado = solicitacoes.Select(MapToDto).ToList();

            _logger.LogInformation("Encontradas {Quantidade} solicitações", resultado.Count);

            return resultado;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar solicitações de ajuste");
            throw;
        }
    }

    public async Task<SolicitacaoAjusteDto?> GetSolicitacaoByIdAsync(Guid id)
    {
        try
        {
            _logger.LogDebug("Buscando solicitação por ID: {SolicitacaoId}", id);

            var solicitacao = await _solicitacaoRepository.GetByIdComFaturamentoAsync(id);
            
            if (solicitacao == null)
            {
                _logger.LogWarning("Solicitação não encontrada. ID: {SolicitacaoId}", id);
                return null;
            }

            _logger.LogDebug("Solicitação encontrada. ID: {SolicitacaoId}, Status: {Status}", 
                id, solicitacao.Status);

            return MapToDto(solicitacao);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar solicitação por ID: {SolicitacaoId}", id);
            throw;
        }
    }

    public async Task<(bool success, string? errorMessage)> RevisarSolicitacaoAsync(
        Guid id,
        string acao,
        Guid aprovadorId)
    {
        using var scope = _logger.BeginScope("Revisando solicitação {SolicitacaoId} por {AprovadorId}", id, aprovadorId);

        try
        {
            // Validações iniciais
            if (aprovadorId == Guid.Empty)
            {
                _logger.LogWarning("Tentativa de revisar solicitação com aprovadorId vazio");
                return (false, "ID do aprovador inválido");
            }

            var acaoNormalizada = acao?.Trim().ToUpper();
            if (string.IsNullOrEmpty(acaoNormalizada))
            {
                _logger.LogWarning("Tentativa de revisar solicitação sem ação especificada");
                return (false, "Ação não especificada");
            }

            if (!ValidarAcaoRevisao(acaoNormalizada))
            {
                _logger.LogWarning("Ação de revisão inválida: {Acao}", acao);
                return (false, $"Ação de revisão inválida: {acao}");
            }

            _logger.LogInformation("Iniciando revisão da solicitação. Ação: {Acao}", acaoNormalizada);

            // Busca a solicitação
            var solicitacao = await _solicitacaoRepository.GetByIdComFaturamentoAsync(id);
            if (solicitacao == null)
            {
                _logger.LogWarning("Solicitação não encontrada para revisão. ID: {SolicitacaoId}", id);
                return (false, ErrorMessages.GenericNotFound);
            }

            // Valida se pode ser revisada
            if (solicitacao.Status != STATUS_PENDENTE)
            {
                _logger.LogWarning("Tentativa de revisar solicitação já processada. Status atual: {Status}", 
                    solicitacao.Status);
                return (false, "Esta solicitação já foi revisada.");
            }

            // Aplica a revisão
            await AplicarRevisaoAsync(solicitacao, acaoNormalizada, aprovadorId);
            
            _logger.LogInformation("Solicitação revisada com sucesso. Status: {Status}, Tipo: {Tipo}", 
                acaoNormalizada, solicitacao.Tipo);

            return (true, null);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Erro ao deserializar dados da solicitação. ID: {SolicitacaoId}", id);
            return (false, $"Erro ao processar os dados da solicitação: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao revisar solicitação. ID: {SolicitacaoId}", id);
            return (false, $"Erro inesperado ao revisar solicitação: {ex.Message}");
        }
    }

    private async Task AplicarRevisaoAsync(
        SolicitacaoAjuste solicitacao,
        string acao,
        Guid aprovadorId)
    {
        _logger.LogDebug("Aplicando revisão. Status: {Status}, Aprovador: {AprovadorId}", acao, aprovadorId);

        // Atualiza informações básicas da solicitação
        solicitacao.Status = acao;
        solicitacao.AprovadorId = aprovadorId;
        solicitacao.DataRevisao = DateTime.UtcNow;

        // Se foi aprovada, aplica as alterações específicas
        if (acao == STATUS_APROVADA)
        {
            _logger.LogDebug("Solicitação aprovada, aplicando alterações específicas. Tipo: {Tipo}", 
                solicitacao.Tipo);
            await AplicarAlteracoesAprovadasAsync(solicitacao);
        }
        else
        {
            _logger.LogDebug("Solicitação não aprovada. Status: {Status}", acao);
        }

        await _solicitacaoRepository.SaveChangesAsync();
        
        _logger.LogDebug("Alterações salvas com sucesso");
    }

    private async Task AplicarAlteracoesAprovadasAsync(SolicitacaoAjuste solicitacao)
    {
        var tipoNormalizado = solicitacao.Tipo?.Trim().ToLower();

        switch (tipoNormalizado)
        {
            case TIPO_REMOCAO:
                _logger.LogInformation("Desativando faturamento parcial para solicitação de remoção. FaturamentoId: {FaturamentoId}", 
                    solicitacao.FaturamentoParcial?.Id);
                solicitacao.FaturamentoParcial!.IsAtivo = false;
                break;

            case TIPO_ALTERACAO when !string.IsNullOrEmpty(solicitacao.DadosNovos):
                _logger.LogInformation("Aplicando alteração de dados. DadosNovos: {DadosNovos}", 
                    solicitacao.DadosNovos);
                await AplicarAlteracaoDeDadosAsync(solicitacao);
                break;

            case TIPO_ADICAO:
                _logger.LogDebug("Solicitação de adição aprovada. Nenhuma alteração específica necessária.");
                break;

            default:
                _logger.LogWarning("Tipo de solicitação não reconhecido para alterações: {Tipo}", 
                    solicitacao.Tipo);
                break;
        }
    }

    private Task AplicarAlteracaoDeDadosAsync(SolicitacaoAjuste solicitacao)
    {
        try
        {
            var dadosNovos = JsonSerializer.Deserialize<DadosNovosDto>(
                solicitacao.DadosNovos!,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    WriteIndented = false // Para log mais compacto
                });

            if (dadosNovos != null)
            {
                _logger.LogDebug("Dados novos deserializados. Valor: {Valor}, Data: {Data}", 
                    dadosNovos.Valor, dadosNovos.Data);

                var valorAntigo = solicitacao.FaturamentoParcial!.Valor;
                var dataAntiga = solicitacao.FaturamentoParcial.HoraInicio;

                solicitacao.FaturamentoParcial.Valor = dadosNovos.Valor;
                solicitacao.FaturamentoParcial.HoraInicio = 
                    DateTime.SpecifyKind(dadosNovos.Data, DateTimeKind.Utc);

                _logger.LogInformation("Dados atualizados. Valor: {ValorAntigo} -> {ValorNovo}, Data: {DataAntiga} -> {DataNova}",
                    valorAntigo, dadosNovos.Valor, dataAntiga, dadosNovos.Data);
            }
            else
            {
                _logger.LogWarning("Falha ao deserializar dados novos. DadosNovos: {DadosNovos}", 
                    solicitacao.DadosNovos);
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Erro ao deserializar DadosNovos. Conteúdo: {DadosNovos}", 
                solicitacao.DadosNovos);
            throw;
        }

        return Task.CompletedTask;
    }

    private SolicitacaoAjusteDto MapToDto(SolicitacaoAjuste solicitacao)
    {
        if (solicitacao == null)
            throw new ArgumentNullException(nameof(solicitacao));

        try
        {
            var dto = new SolicitacaoAjusteDto
            {
                Id = solicitacao.Id,
                Status = solicitacao.Status,
                Tipo = solicitacao.Tipo,
                Motivo = solicitacao.Motivo,
                DadosAntigos = solicitacao.DadosAntigos,
                DadosNovos = solicitacao.DadosNovos,
                DataSolicitacao = solicitacao.DataSolicitacao
            };

            // Mapeia o faturamento parcial se existir
            if (solicitacao.FaturamentoParcial != null)
            {
                dto.Faturamento = new FaturamentoSimplesDto
                {
                    Id = solicitacao.FaturamentoParcial.Id,
                    Data = solicitacao.FaturamentoParcial.HoraInicio,
                    Valor = solicitacao.FaturamentoParcial.Valor
                };

                // Mapeia a operação se a navegação estiver disponível
                if (solicitacao.FaturamentoParcial.FaturamentoDiario?.Unidade != null)
                {
                    dto.Faturamento.Operacao = new OperacaoSimplesDto
                    {
                        Id = solicitacao.FaturamentoParcial.FaturamentoDiario.Unidade.Id,
                        Nome = solicitacao.FaturamentoParcial.FaturamentoDiario.Unidade.Nome
                    };
                }
            }

            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao mapear solicitação para DTO. SolicitacaoId: {SolicitacaoId}", 
                solicitacao.Id);
            throw;
        }
    }

    private bool ValidarTipoSolicitacao(string? tipo)
    {
        if (string.IsNullOrEmpty(tipo))
            return false;

        var tiposValidos = new[] { TIPO_REMOCAO, TIPO_ALTERACAO, TIPO_ADICAO };
        return tiposValidos.Contains(tipo.ToLower());
    }

    private bool ValidarAcaoRevisao(string acao)
    {
        var acoesValidas = new[] { STATUS_APROVADA, STATUS_REJEITADA, STATUS_CANCELADA };
        return acoesValidas.Contains(acao);
    }
}

// DTO auxiliar para deserialização dos dados novos
public class DadosNovosDto
{
    public decimal Valor { get; set; }
    public DateTime Data { get; set; }
}