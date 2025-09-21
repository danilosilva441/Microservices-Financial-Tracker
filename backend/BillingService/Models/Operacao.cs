using System.ComponentModel.DataAnnotations;
namespace BillingService.Models;
public class Operacao
{
    public Guid Id { get; set; }

    public decimal? ProjecaoFaturamento { get; set; }
    public ICollection<Faturamento> Faturamentos { get; set; } = new List<Faturamento>();
    [Required]
    public required string Nome { get; set; }
    public string? Descricao { get; set; }
    public string? Endereco { get; set; }
    [Required]
    public required string Moeda { get; set; }
    public decimal MetaMensal { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public bool IsAtiva { get; set; }
    public Guid UserId { get; set; }
    
}