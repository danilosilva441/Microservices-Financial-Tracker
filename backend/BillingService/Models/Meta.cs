using System.ComponentModel.DataAnnotations;

namespace BillingService.Models
{
    public class Meta : BaseEntity
    {
        [Required]
        public Guid UserId { get; set; } // O Dono da meta (Já era Guid)

        [Required]
        public int Mes { get; set; }

        [Required]
        public int Ano { get; set; }

        [Required]
        public decimal ValorAlvo { get; set; }
    }
}