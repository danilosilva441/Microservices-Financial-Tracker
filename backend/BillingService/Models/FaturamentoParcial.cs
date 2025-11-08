// Caminho: backend/BillingService/Models/FaturamentoParcial.cs
// (Arquivo Faturamento.cs renomeado)
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BillingService.Models; // 1. IMPORTANTE: Adiciona o using local

namespace BillingService.Models
{
    // 2. MUDANÇA: Renomeado de Faturamento para FaturamentoParcial
    // 3. MUDANÇA: Herda do BaseEntity local (para ganhar Id e TenantId)
    public class FaturamentoParcial : BaseEntity 
    {
        [Required]
        public decimal Valor { get; set; }

        // 4. MUDANÇA: O campo 'Data' (v1.0) foi removido
        [Required]
        public DateTime HoraInicio { get; set; } // v2.0

        [Required]
        public DateTime HoraFim { get; set; } // v2.0

        // --- 5. MUDANÇA CRÍTICA (A Relação v2.0) ---
        // O FaturamentoParcial (item) agora pertence a
        // um FaturamentoDiario (o "cabeçalho" do dia).
        [Required]
        public Guid FaturamentoDiarioId { get; set; }

        [ForeignKey("FaturamentoDiarioId")]
        public virtual FaturamentoDiario FaturamentoDiario { get; set; } = null!;

        // --- 6. REMOVIDO (v1.0) ---
        // public Guid OperacaoId { get; set; }
        // public Operacao Operacao { get; set; } = null!;
        // (A Operacao/Unidade agora é acessada através do FaturamentoDiario.Unidade)

        // --- 7. NOVO (v2.0 - vindo dos formulários) ---
        [Required]
        public Guid MetodoPagamentoId { get; set; }
        
        [ForeignKey("MetodoPagamentoId")]
        public virtual MetodoPagamento MetodoPagamento { get; set; } = null!;

        [Required]
        public string Origem { get; set; } = "AVULSO";
        
        public bool IsAtivo { get; set; } = true;
    }
}