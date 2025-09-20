using System.ComponentModel.DataAnnotations;

namespace BillingService.Models
{
    public class Meta
    {
        public Guid Id { get; set; }
        public int Mes { get; set; }
        public int Ano { get; set; }

        [Required]
        public decimal ValorAlvo { get; set; }
        
        // Chave estrangeira para o usuário
        public Guid UserId { get; set; }
    }
}