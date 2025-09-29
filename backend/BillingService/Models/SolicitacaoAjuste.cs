using System.ComponentModel.DataAnnotations;

namespace BillingService.Models;

public class SolicitacaoAjuste
{
    public Guid Id { get; set; }

    [Required]
    public Guid FaturamentoId { get; set; }
    public Faturamento Faturamento { get; set; } = null!;

    [Required]
    public Guid SolicitanteId { get; set; } // O UserId de quem pediu

    [Required]
    public required string Tipo { get; set; } // "alteracao" ou "remocao"

    [Required]
    public required string Motivo { get; set; }

    public string? DadosAntigos { get; set; } // JSON com os dados originais
    public string? DadosNovos { get; set; }   // JSON com os dados propostos

    [Required]
    public required string Status { get; set; } = "PENDENTE"; // PENDENTE, APROVADA, REJEITADA

    public Guid? AprovadorId { get; set; } // O UserId do Admin que revisou
    public DateTime DataSolicitacao { get; set; } = DateTime.UtcNow;
    public DateTime? DataRevisao { get; set; }
}