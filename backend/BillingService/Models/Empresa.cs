using System.ComponentModel.DataAnnotations;

namespace BillingService.Models;

public class Empresa
{
    public Guid Id { get; set; }
    [Required]
    public required string Nome { get; set; }
    [Required]
    public required string CNPJ { get; set; }
    public int DiaVencimentoBoleto { get; set; } = 5;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}