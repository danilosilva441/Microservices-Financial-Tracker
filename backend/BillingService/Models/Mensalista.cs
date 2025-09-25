using System.ComponentModel.DataAnnotations;

namespace BillingService.Models;

public class Mensalista
{
    public Guid Id { get; set; }
    [Required]
    public required string Nome { get; set; }
    public string? CPF { get; set; }
    public decimal ValorMensalidade { get; set; }
    public bool IsAtivo { get; set; } = true;

    // Chaves Estrangeiras
    public Guid OperacaoId { get; set; }
    public Operacao Operacao { get; set; } = null!;

    public Guid? EmpresaId { get; set; } // Anulável, para mensalistas pessoa física
    public Empresa? Empresa { get; set; }
}