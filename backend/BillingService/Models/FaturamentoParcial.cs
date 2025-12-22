// Caminho: backend/BillingService/Models/FaturamentoParcial.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BillingService.Models; // 1. MUDANÇA: Usando o namespace local
using SharedKernel.Entities;
using SharedKernel.Enums;

namespace BillingService.Models
{
    // 2. MUDANÇA: Herda do BaseEntity local (para ganhar Id e TenantId)
    public class FaturamentoParcial : BaseEntity 
    {
        [Required]
        public decimal Valor { get; set; }

        [Required]
        public DateTime HoraInicio { get; set; } 

        [Required]
        public DateTime HoraFim { get; set; }

        // --- 3. MUDANÇA CRÍTICA (A Relação) ---
        // O FaturamentoParcial (item) agora pertence a
        // um FaturamentoDiario (o cabeçalho do dia).
        [Required]
        public Guid FaturamentoDiarioId { get; set; }

        [ForeignKey("FaturamentoDiarioId")]
        public virtual FaturamentoDiario FaturamentoDiario { get; set; } = null!;

        // --- 4. REMOVIDO ---
        // public Guid OperacaoId { get; set; }
        // public Operacao Operacao { get; set; } = null!;
        // (A Operacao/Unidade agora é acessada através do FaturamentoDiario.Unidade)

        
        [ForeignKey("MetodoPagamentoId")]
        public MetodoPagamentoEnum MetodoPagamentoId { get; set; }

        [Required]
        public string Origem { get; set; } = "AVULSO";
        
        public bool IsAtivo { get; set; } = true;
    }
}