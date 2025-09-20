// Caminho: BillingService/Models/Operacao.cs
using System.ComponentModel.DataAnnotations;

namespace BillingService.Models;

public class Operacao
{
    public Guid Id { get; set; }
    public required string Descricao { get; set; }
    public decimal MetaMensal { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public bool IsAtiva { get; set; }
    public Guid UserId { get; set; }

    // Coleção de Faturamentos vinculados
    public ICollection<Faturamento> Faturamentos { get; set; } = new List<Faturamento>();
}