using System.ComponentModel.DataAnnotations;

namespace BillingService.DTOs
{
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

        [Required] // ✅ ADICIONADO
        public string Origem { get; set; } = "AVULSO";
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