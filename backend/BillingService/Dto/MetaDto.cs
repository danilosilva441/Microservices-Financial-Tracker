// Caminho: backend/BillingService/DTOs/MetaDto.cs
using System.ComponentModel.DataAnnotations;

// para corresponder ao resto do projeto.
namespace BillingService.DTOs
{
    public class MetaDto
    {
        [Required]
        [Range(1, 12)]
        public int Mes { get; set; }

        [Required]
        [Range(2020, 2050)]
        public int Ano { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal ValorAlvo { get; set; }
    }
}