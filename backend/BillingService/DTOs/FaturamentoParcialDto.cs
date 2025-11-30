// Caminho: backend/BillingService/DTOs/FaturamentoParcialDto.cs
using System.ComponentModel.DataAnnotations;

// Garanta que o namespace é DTOs (plural)
namespace BillingService.DTOs
{
    // DTO para CRIAR um FaturamentoParcial (v2.0)
    public class FaturamentoParcialCreateDto
    {
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Valor { get; set; }

        [Required]
        public DateTime HoraInicio { get; set; }

        [Required]
        public DateTime HoraFim { get; set; }

        [Required]
        public Guid MetodoPagamentoId { get; set; }

        [Required]
        public string Origem { get; set; } = "AVULSO";
    }

    // DTO para ATUALIZAR um FaturamentoParcial (v2.0)
    public class FaturamentoParcialUpdateDto
    {
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Valor { get; set; }
        
        [Required]
        public DateTime HoraInicio { get; set; }
        
        [Required]
        public DateTime HoraFim { get; set; }
        
        [Required]
        public Guid MetodoPagamentoId { get; set; }
    }

    public class FaturamentoParcialResponseDto
    {
    public Guid Id { get; set; }
    public decimal Valor { get; set; }
    public DateTime HoraInicio { get; set; }
    public DateTime HoraFim { get; set; }
    public Guid MetodoPagamentoId { get; set; }
    public string Origem { get; set; } = string.Empty;
    public bool IsAtivo { get; set; }
    }
}