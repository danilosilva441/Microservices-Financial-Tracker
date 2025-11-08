// Caminho: backend/BillingService/Models/Expense.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingService.Models
{
    // 1. Também herda de BaseEntity (ganhando TenantId).
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

        // Relação com a Unidade (ex-Operacao)
        // Para qual unidade esta despesa pertence?
        [Required]
        // 2. IMPORTANTE: Já vamos usar o nome v2.0 "UnidadeId"
        public Guid UnidadeId { get; set; } 
        
        [ForeignKey("UnidadeId")]
        // 3. IMPORTANTE: Já vamos usar o nome v2.0 "Unidade"
        public virtual Unidade Unidade { get; set; } = null!; 
    }
}