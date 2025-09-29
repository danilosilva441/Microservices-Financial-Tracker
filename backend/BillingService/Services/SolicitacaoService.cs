using BillingService.DTOs;
using BillingService.Models;
using BillingService.Repositories;
using System.Text.Json;

namespace BillingService.Services;

public class SolicitacaoService
{
    private readonly SolicitacaoRepository _repository;
    private readonly FaturamentoRepository _faturamentoRepository;

    public SolicitacaoService(SolicitacaoRepository repository, FaturamentoRepository faturamentoRepository)
    {
        _repository = repository;
        _faturamentoRepository = faturamentoRepository;
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
                Id = s.Faturamento.Id,
                // Atribui os valores corretos do faturamento
                Data = s.Faturamento.Data,
                Valor = s.Faturamento.Valor,
                Operacao = new OperacaoSimplesDto
                {
                    Id = s.Faturamento.Operacao.Id,
                    Nome = s.Faturamento.Operacao.Nome
                }
            }
        });

    }

    public async Task<(bool success, string? errorMessage)> RevisarSolicitacaoAsync(Guid id, string acao, Guid aprovadorId)
    {
        var solicitacao = await _repository.GetByIdComFaturamentoAsync(id);
        if (solicitacao == null) return (false, "Solicitação não encontrada.");
        if (solicitacao.Status != "PENDENTE") return (false, "Esta solicitação já foi revisada.");

        solicitacao.Status = acao.ToUpper();
        solicitacao.AprovadorId = aprovadorId;
        solicitacao.DataRevisao = DateTime.UtcNow;

        if (solicitacao.Status == "APROVADA")
        {
            if (solicitacao.Tipo.ToLower() == "remocao")
            {
                solicitacao.Faturamento.IsAtivo = false;
            }
            else if (solicitacao.Tipo.ToLower() == "alteracao" && !string.IsNullOrEmpty(solicitacao.DadosNovos))
            {
                // --- CORREÇÃO AQUI ---
                // Usa o novo DTO para desserializar com segurança
                var dadosNovos = JsonSerializer.Deserialize<DadosNovosDto>(solicitacao.DadosNovos);
                if (dadosNovos != null)
                {
                    solicitacao.Faturamento.Valor = dadosNovos.Valor;
                    solicitacao.Faturamento.Data = DateTime.SpecifyKind(dadosNovos.Data, DateTimeKind.Utc);
                }
            }
        }

        await _repository.SaveChangesAsync();
        return (true, null);
    }
}