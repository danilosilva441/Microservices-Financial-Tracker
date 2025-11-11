// Caminho: backend/BillingService/DTOs/ProjecaoDto.cs
using System.ComponentModel.DataAnnotations;

namespace BillingService.DTOs
{
    // DTO v1.0 (mantido para o endpoint de projeção)
    public class ProjecaoDto
    {
        [Required]
        [Range(0, double.MaxValue)]
        public decimal ProjecaoFaturamento { get; set; }
    }
}