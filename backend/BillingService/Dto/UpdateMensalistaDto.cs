using System.ComponentModel.DataAnnotations;

namespace BillingService.DTO;

public class UpdateMensalistaDto
{
    [Required]
    public required string Nome { get; set; }
    public string? CPF { get; set; }
    [Required]
    public decimal ValorMensalidade { get; set; }
    public bool IsAtivo { get; set; }
    public Guid? EmpresaId { get; set; }
}