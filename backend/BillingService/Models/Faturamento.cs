// Caminho: BillingService/Models/Faturamento.cs
using System.ComponentModel.DataAnnotations;

namespace BillingService.Models;

public class Faturamento
{
    public Guid Id { get; set; }
    public decimal Valor { get; set; }
    public DateTime Data { get; set; }

    // Chave estrangeira para vincular à Operacao
    public Guid OperacaoId { get; set; }
    public Operacao Operacao { get; set; } = null!; // Propriedade de navegação
}