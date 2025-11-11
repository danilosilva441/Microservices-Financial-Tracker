using System.ComponentModel.DataAnnotations;

namespace BillingService.DTO;

public class UpdateEmpresaDto
{
    [Required]
    public required string Nome { get; set; }
    [Required]
    public required string CNPJ { get; set; }
    [Range(1, 28)]
    public int DiaVencimentoBoleto { get; set; }
}