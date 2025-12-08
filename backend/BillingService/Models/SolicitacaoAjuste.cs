using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SharedKernel.Entities;

namespace BillingService.Models
{
    public class SolicitacaoAjuste : BaseEntity
    {
        // Id e TenantId vêm da BaseEntity

        // MUDANÇA: Renomeado de FaturamentoId para FaturamentoParcialId
        [Required]
        public Guid FaturamentoParcialId { get; set; }
        
        [ForeignKey("FaturamentoParcialId")]
        public FaturamentoParcial FaturamentoParcial { get; set; } = null!;

        [Required]
        public Guid SolicitanteId { get; set; } // O UserId de quem pediu (Já era Guid)

        [Required]
        public string Tipo { get; set; } = "alteracao"; // "alteracao" ou "remocao"

        [Required]
        public string Motivo { get; set; } = null!;

        public string? DadosAntigos { get; set; } // JSON
        public string? DadosNovos { get; set; }   // JSON

        [Required]
        public string Status { get; set; } = "PENDENTE"; // PENDENTE, APROVADA, REJEITADA

        public Guid? AprovadorId { get; set; } // O UserId do Admin (Já era Guid)
        public DateTime DataSolicitacao { get; set; } = DateTime.UtcNow;
        public DateTime? DataRevisao { get; set; }
    }
}