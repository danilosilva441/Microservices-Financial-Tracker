// Caminho: backend/BillingService/DTOs/CriarSolicitacaoDto.cs
using System.ComponentModel.DataAnnotations;

namespace BillingService.DTOs
{
    public class CriarSolicitacaoDto
    {
        // 1. MUDANÇA (v2.0): Renomeado de FaturamentoId
        [Required]
        public Guid FaturamentoParcialId { get; set; }

        [Required]
        public string Tipo { get; set; } = "alteracao"; // "alteracao" ou "remocao"

        [Required]
        public string Motivo { get; set; } = null!;

        public string? DadosAntigos { get; set; } // JSON
        public string? DadosNovos { get; set; }   // JSON
    }
}