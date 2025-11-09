using System.ComponentModel.DataAnnotations;

namespace BillingService.DTO
{
    // DTO para CRIAR um FaturamentoParcial (substitui o FaturamentoDto)
    public class FaturamentoParcialCreateDto
    {
        [Required]
        public decimal Valor { get; set; }

        [Required]
        public DateTime HoraInicio { get; set; } // NOVO (substitui 'Data')

        [Required]
        public DateTime HoraFim { get; set; } // NOVO

        [Required]
        public Guid MetodoPagamentoId { get; set; } // NOVO

        public string Origem { get; set; } = "AVULSO";
    }

    // DTO para ATUALIZAR um FaturamentoParcial (substitui o UpdateFaturamentoDto)
    public class FaturamentoParcialUpdateDto
    {
        [Required]
        public decimal Valor { get; set; }
        
        [Required]
        public DateTime HoraInicio { get; set; } // NOVO
        
        [Required]
        public DateTime HoraFim { get; set; } // NOVO
        
        [Required]
        public Guid MetodoPagamentoId { get; set; } // NOVO
    }
}