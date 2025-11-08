// Caminho: backend/BillingService/Models/MetodoPagamento.cs
using System.ComponentModel.DataAnnotations;
using BillingService.Models; // 1. IMPORTANTE: Adiciona o using local

namespace BillingService.Models
{
    // 2. Herda do BaseEntity (ganha Id e TenantId)
    public class MetodoPagamento : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Nome { get; set; } = string.Empty; // Ex: "PIX", "DINHEIRO", "CRÉDITO"

        [MaxLength(255)]
        public string? Descricao { get; set; }

        // Propriedade de Navegação
        // Um método de pagamento pode ser usado em muitos faturamentos parciais
        public virtual ICollection<FaturamentoParcial> FaturamentosParciais { get; set; } = new List<FaturamentoParcial>();
    }
}