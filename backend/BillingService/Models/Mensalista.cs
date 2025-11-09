// Caminho: backend/BillingService/Models/Mensalista.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BillingService.Models; // Garante que ele veja o BaseEntity e Unidade

namespace BillingService.Models
{
    // Herda do BaseEntity local (para ganhar Id e TenantId)
    public class Mensalista : BaseEntity 
    {
        [Required]
        public string Nome { get; set; } = null!;

        public string? CPF { get; set; }
        
        [Required]
        public decimal ValorMensalidade { get; set; }
        
        public bool IsAtivo { get; set; } = true;

        // --- 1. MUDANÇA (Roadmap v2.0) ---
        // Renomeado de OperacaoId para UnidadeId
        [Required]
        public Guid UnidadeId { get; set; }
        
        // --- 2. MUDANÇA (Roadmap v2.0) ---
        [ForeignKey("UnidadeId")]
        public virtual Unidade Unidade { get; set; } = null!; // Renomeado para Unidade
    }
}