using System.ComponentModel.DataAnnotations;

namespace BillingService.Models;

public class Fatura
{
    public Guid Id { get; set; }
    public int MesReferencia { get; set; }
    public int AnoReferencia { get; set; }
    public decimal ValorTotal { get; set; }
    public DateTime DataVencimento { get; set; }
    [Required]
    public required string Status { get; set; } = "PENDENTE"; // PENDENTE, PAGO, ATRASADO
    public DateTime? DataPagamento { get; set; }

    // Chave Estrangeira
    public Guid EmpresaId { get; set; }
    public Empresa Empresa { get; set; } = null!;
}