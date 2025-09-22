using System.ComponentModel.DataAnnotations;
namespace BillingService.DTO;
public class UpdateOperacaoDto
{
    [Required]
    public required string Nome { get; set; }
    public string? Descricao { get; set; }
    public string? Endereco { get; set; }
    [Required]
    public required string Moeda { get; set; }
    [Required]
    public decimal MetaMensal { get; set; }
    [Required]
    public DateTime DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
}