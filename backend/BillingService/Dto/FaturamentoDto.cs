using System.ComponentModel.DataAnnotations;

namespace BillingService.DTO;

public class CreateFaturamentoDto
{
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser positivo.")]
    public decimal Valor { get; set; }

    [Required]
    public DateTime Data { get; set; }
}