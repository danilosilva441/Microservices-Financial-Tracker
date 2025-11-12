using BillingService.DTOs;
using BillingService.Models;
using BillingService.Repositories.Interfaces; // <-- 1. MUDANÇA: Adiciona as Interfaces
using BillingService.Services.Interfaces; // <-- 1. MUDANÇA: Adiciona as Interfaces
using System.Text.Json;
using SharedKernel;

namespace BillingService.Services;

public class SolicitacaoService : ISolicitacaoService // <-- Herda da Interface
{
    // 2. MUDANÇA: Injetando INTERFACES v2.0
    private readonly ISolicitacaoRepository _repository;
    private readonly IFaturamentoParcialRepository _faturamentoRepository;
    private readonly IUnidadeRepository _unidadeRepository;

    public SolicitacaoService(
        ISolicitacaoRepository repository,
        IFaturamentoParcialRepository faturamentoRepository,
        IUnidadeRepository unidadeRepository // <-- 3. MUDANÇA: Injeta a interface v2.0
    )
    {
        _repository = repository;
        _faturamentoRepository = faturamentoRepository;
        _unidadeRepository = unidadeRepository;
    }

    public async Task<SolicitacaoAjuste> CriarSolicitacaoAsync(SolicitacaoAjuste solicitacao, Guid solicitanteId)
    {
        solicitacao.SolicitanteId = solicitanteId;
        solicitacao.Status = "PENDENTE";

        await _repository.AddAsync(solicitacao);
        await _repository.SaveChangesAsync();
        return solicitacao;
    }

    public async Task<IEnumerable<SolicitacaoAjusteDto>> GetSolicitacoesAsync()
    {
        var solicitacoes = await _repository.GetAllComDetalhesAsync();

        // Mapeia a entidade para o DTO
        return solicitacoes.Select(s => new SolicitacaoAjusteDto
        {
            Id = s.Id,
            Status = s.Status,
            Tipo = s.Tipo,
            Motivo = s.Motivo,
            DadosAntigos = s.DadosAntigos,
            DadosNovos = s.DadosNovos,
            DataSolicitacao = s.DataSolicitacao,
            Faturamento = new FaturamentoSimplesDto
            {
                Id = s.FaturamentoParcial.Id,
                Data = s.FaturamentoParcial.HoraInicio,
                Valor = s.FaturamentoParcial.Valor,
                Operacao = new OperacaoSimplesDto
                {
                    // 4. MUDANÇA: Corrigindo o caminho da relação v2.0
                    Id = s.FaturamentoParcial.FaturamentoDiario.Unidade.Id,
                    Nome = s.FaturamentoParcial.FaturamentoDiario.Unidade.Nome
                }
            }
        });
    }

    public async Task<(bool success, string? errorMessage)> RevisarSolicitacaoAsync(Guid id, string acao, Guid aprovadorId)
    {
        var solicitacao = await _repository.GetByIdComFaturamentoAsync(id);
        if (solicitacao == null) return (false, ErrorMessages.GenericNotFound);
        if (solicitacao.Status != "PENDENTE") return (false, "Esta solicitação já foi revisada.");

        solicitacao.Status = acao.ToUpper();
        solicitacao.AprovadorId = aprovadorId;
        solicitacao.DataRevisao = DateTime.UtcNow;

        if (solicitacao.Status == "APROVADA")
        {
            if (solicitacao.Tipo.ToLower() == "remocao")
            {
                solicitacao.FaturamentoParcial.IsAtivo = false;
            }
            else if (solicitacao.Tipo.ToLower() == "alteracao" && !string.IsNullOrEmpty(solicitacao.DadosNovos))
            {
                var dadosNovos = JsonSerializer.Deserialize<DadosNovosDto>(solicitacao.DadosNovos);
                if (dadosNovos != null)
                {
                    solicitacao.FaturamentoParcial.Valor = dadosNovos.Valor;
                    solicitacao.FaturamentoParcial.HoraInicio = DateTime.SpecifyKind(dadosNovos.Data, DateTimeKind.Utc);
                }
            }
        }

        await _repository.SaveChangesAsync();
        return (true, null);
    }
}