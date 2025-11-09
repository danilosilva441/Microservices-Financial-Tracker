using System.ComponentModel.DataAnnotations;

namespace BillingService.Models
{
    // Esta é a nova tabela para métodos de pagamento (Tarefa 1.2)
    public class MetodoPagamento : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = null!; // Ex: "Pix", "Cartão", "Dinheiro"

        public bool IsAtivo { get; set; } = true;
    }
}