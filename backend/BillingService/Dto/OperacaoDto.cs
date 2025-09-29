using System.ComponentModel.DataAnnotations;
namespace BillingService.DTO;

public class OperacaoDto
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public required string Nome { get; set; }
    public string? Descricao { get; set; }
    public string? Endereco { get; set; }
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal MetaMensal { get; set; }
    [Required]
    public DateTime DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
}