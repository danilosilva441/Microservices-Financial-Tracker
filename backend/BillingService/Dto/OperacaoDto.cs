using System.ComponentModel.DataAnnotations;

namespace BillingService.DTO;

public class CreateOperacaoDto
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public required string Descricao { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal MetaMensal { get; set; } // <--- MUDANÇA AQUI (de Valor para MetaMensal)

    [Required]
    public DateTime DataInicio { get; set; }

    public DateTime? DataFim { get; set; }
}