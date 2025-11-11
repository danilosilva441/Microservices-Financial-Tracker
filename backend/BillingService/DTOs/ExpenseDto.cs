// Caminho: backend/BillingService/DTOs/ExpenseDto.cs
using System.ComponentModel.DataAnnotations;

namespace BillingService.DTOs
{
    // DTO para criar uma Categoria de Despesa
    public class ExpenseCategoryCreateDto
    {
        [Required(ErrorMessage = "O nome da categoria é obrigatório.")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Description { get; set; }
    }

    // DTO para criar um lançamento de Despesa

    public class ExpenseCreateDto
    {
        [Required(ErrorMessage = "O valor é obrigatório.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser positivo.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "A data é obrigatória.")]
        public DateTime ExpenseDate { get; set; }

        [MaxLength(255)]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "A categoria é obrigatória.")]
        public Guid CategoryId { get; set; }

        // --- MUDANÇA (v2.0) ---
        // Renomeado de OperacaoId para UnidadeId
        [Required(ErrorMessage = "A unidade é obrigatória.")]
        public Guid UnidadeId { get; set; }
    }
}