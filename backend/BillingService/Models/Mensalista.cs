// Caminho: backend/BillingService/Models/Mensalista.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BillingService.Models; // 1. IMPORTANTE: Adiciona o using local

namespace BillingService.Models
{
    // 2. MUDANÇA: Herda do BaseEntity local (para ganhar Id e TenantId)
    public class Mensalista : BaseEntity 
    {
        [Required]
        public string Nome { get; set; } = null!;

        public string? CPF { get; set; }
        
        [Required]
        public decimal ValorMensalidade { get; set; }
        
        public bool IsAtivo { get; set; } = true;

        // --- 3. MUDANÇA (v2.0) ---
        // Renomeado de OperacaoId para UnidadeId
        [Required]
        public Guid UnidadeId { get; set; }
        
        // --- 4. MUDANÇA (v2.0) ---
        [ForeignKey("UnidadeId")]
        public virtual Unidade Unidade { get; set; } = null!; // Renomeado para Unidade
    }
}