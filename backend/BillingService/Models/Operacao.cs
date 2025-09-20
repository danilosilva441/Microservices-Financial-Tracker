using System.ComponentModel.DataAnnotations;

namespace BillingService.Models;

public class Operacao
{
    public Guid Id { get; set; }

    [Required]
    public required string Descricao { get; set; } // <--- CORREÇÃO 1: Adicionado 'required'

    [Required]
    public decimal Valor { get; set; }

    public DateTime DataInicio { get; set; }

    public DateTime? DataFim { get; set; } // <--- CORREÇÃO 2: Adicionado '?' para permitir nulos

    public bool IsAtiva { get; set; } // <--- CORREÇÃO 3: Adicionada a propriedade que faltava

    // Chave estrangeira para o usuário
    public Guid UserId { get; set; }
}