// Caminho: backend/BillingService/Models/SolicitacaoAjuste.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BillingService.Models; // 1. IMPORTANTE: Adiciona o using local

namespace BillingService.Models
{
    // 2. MUDANÇA: Herda do BaseEntity (ganha Id e TenantId)
    public class SolicitacaoAjuste : BaseEntity 
    {
        // Id e TenantId vêm da BaseEntity

        // 3. MUDANÇA: Renomeado de FaturamentoId
        [Required]
        public Guid FaturamentoParcialId { get; set; } 
        
        // 4. MUDANÇA: Renomeado de Faturamento
        [ForeignKey("FaturamentoParcialId")]
        public virtual FaturamentoParcial FaturamentoParcial { get; set; } = null!; 

        [Required]
        public Guid SolicitanteId { get; set; } 

        [Required]
        public string Tipo { get; set; } = "alteracao"; // "alteracao" ou "remocao"

        [Required]
        public string Motivo { get; set; } = null!;

        public string? DadosAntigos { get; set; } // JSON
        public string? DadosNovos { get; set; }   // JSON

        [Required]
        public string Status { get; set; } = "PENDENTE"; // PENDENTE, APROVADA, REJEITADA

        public Guid? AprovadorId { get; set; } 
        public DateTime DataSolicitacao { get; set; } = DateTime.UtcNow;
        public DateTime? DataRevisao { get; set; }
    }
}