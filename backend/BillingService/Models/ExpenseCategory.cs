// Caminho: backend/BillingService/Models/ExpenseCategory.cs
using System.ComponentModel.DataAnnotations;

namespace BillingService.Models
{
    // 1. Note que ela herda de BaseEntity.
    //    Isso significa que ela já tem Id, TenantId, CreatedAt, etc.
    public class ExpenseCategory : BaseEntity 
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Description { get; set; }

        // Relação (Uma categoria pode ter muitas despesas)
        public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }
}