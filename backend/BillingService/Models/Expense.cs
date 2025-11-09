// Caminho: backend/BillingService/Models/Expense.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BillingService.Models; // Adicionando o using local

namespace BillingService.Models
{
    // 1. Herança do BaseEntity local (Já estava correto)
    public class Expense : BaseEntity
    {
        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTime ExpenseDate { get; set; } // Data da despesa

        [MaxLength(255)]
        public string Description { get; set; } = string.Empty;

        // Relação com a Categoria
        [Required]
        public Guid CategoryId { get; set; }
        
        [ForeignKey("CategoryId")]
        public virtual ExpenseCategory Category { get; set; } = null!;

        // --- 2. MUDANÇA (Roadmap v2.0) ---
        // Renomeado de OperacaoId para UnidadeId
        [Required]
        public Guid UnidadeId { get; set; }
        
        // --- 3. MUDANÇA (Roadmap v2.0) ---
        [ForeignKey("UnidadeId")]
        public virtual Unidade Unidade { get; set; } = null!; // Renomeado para Unidade
    }
}