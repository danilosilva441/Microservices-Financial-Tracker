using System.ComponentModel.DataAnnotations;

namespace BillingService.DTO;

public class UpdateFaturamentoDto
{
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Valor { get; set; }

    [Required]
    public DateTime Data { get; set; }
}