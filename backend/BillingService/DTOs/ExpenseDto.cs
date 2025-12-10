// BillingService/DTOs/ExpenseDto.cs (adicione estas classes)
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

    // DTO para retornar uma Categoria de Despesa
    public class ExpenseCategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid TenantId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
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

    // DTO para retornar um lançamento de Despesa
    public class ExpenseDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public Guid UnidadeId { get; set; }
        public string UnidadeName { get; set; } = string.Empty;
        public Guid TenantId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }

    // DTO para resultado de importação
    public class ImportResultDto
    {
        public int ProcessedRows { get; set; }
        public List<SkippedRowDto> SkippedRows { get; set; } = new();
    }

    public class SkippedRowDto
    {
        public int RowNumber { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    // DTO para resposta do upload
    public class UploadResultDto
    {
        public string Message { get; set; } = string.Empty;
        public int ProcessedRows { get; set; }
        public int TotalRows { get; set; }
        public List<SkippedRowDto> SkippedRows { get; set; } = new();
    }
}