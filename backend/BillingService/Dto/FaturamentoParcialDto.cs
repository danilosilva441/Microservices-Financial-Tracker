// Caminho: backend/BillingService/DTOs/FaturamentoParcialDto.cs
using System.ComponentModel.DataAnnotations;

namespace BillingService.DTOs
{
    // DTO para CRIAR um FaturamentoParcial
    public class FaturamentoParcialCreateDto
    {
        [Required]
        public decimal Valor { get; set; }

        [Required]
        public DateTime HoraInicio { get; set; }

        [Required]
        public DateTime HoraFim { get; set; }

        [Required]
        public Guid MetodoPagamentoId { get; set; }

        public string Origem { get; set; } = "AVULSO";
    }

    // DTO para ATUALIZAR um FaturamentoParcial
    public class FaturamentoParcialUpdateDto
    {
        [Required]
        public decimal Valor { get; set; }
        
        [Required]
        public DateTime HoraInicio { get; set; }
        
        [Required]
        public DateTime HoraFim { get; set; }
        
        [Required]
        public Guid MetodoPagamentoId { get; set; }
    }
}