using System.ComponentModel.DataAnnotations;

namespace BillingService.DTO;

public class CreateMensalistaDto
{
    [Required]
    public required string Nome { get; set; }
    public string? CPF { get; set; }
    [Required]
    public decimal ValorMensalidade { get; set; }
    public Guid? EmpresaId { get; set; } // Opcional
}