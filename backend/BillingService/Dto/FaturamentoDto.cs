using System.ComponentModel.DataAnnotations;
namespace BillingService.DTO;
public class FaturamentoDto // Ou CreateFaturamentoDto
{
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Valor { get; set; }
    [Required]
    public DateTime Data { get; set; }
    [Required]
    public required string Moeda { get; set; } // <-- Adicione/confirme esta linha
}