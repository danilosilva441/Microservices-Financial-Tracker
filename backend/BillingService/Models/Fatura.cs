using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SharedKernel.Entities;

namespace BillingService.Models
{
    public class Fatura : BaseEntity
    {
        public int MesReferencia { get; set; }
        public int AnoReferencia { get; set; }
        
        [Required]
        public decimal ValorTotal { get; set; }
        
        public DateTime DataVencimento { get; set; }
        
        [Required]
        public string Status { get; set; } = "PENDENTE"; // PENDENTE, PAGO, ATRASADO
        
        public DateTime? DataPagamento { get; set; }

        // MUDANÇA: Propriedade de navegação 'Empresa' removida
    }
}