// Caminho: backend/BillingService/Models/FaturamentoDiario.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BillingService.Models; // 1. IMPORTANTE: Usando o namespace local

namespace BillingService.Models
{
    // (Boa prática) Definir os status como um Enum
    // Isso evita erros de digitação (ex: "PENDENTE" vs "Pendente")
    public enum RegistroStatus
    {
        Pendente,  // Submetido pelo Líder/Operador
        Aprovado,  // Aprovado pelo Supervisor
        Rejeitado  // Rejeitado pelo Supervisor
    }
    
    // 2. Herda do BaseEntity local (para ganhar Id e TenantId)
    public class FaturamentoDiario : BaseEntity 
    {
        // 3. Link para a "Unidade" (novo nome)
        [Required]
        public Guid UnidadeId { get; set; } 
        
        [ForeignKey("UnidadeId")]
        public virtual Unidade Unidade { get; set; } = null!;

        [Required]
        public DateOnly Data { get; set; } // A data consolidada

        // 4. Usa o Enum para Status (mais seguro)
        [Required]
        public RegistroStatus Status { get; set; } = RegistroStatus.Pendente;

        
        // --- 5. NOVOS CAMPOS (Baseado nos formulários de papel / Roadmap 4.1) ---

        // Campos preenchidos pelo "Líder" (Operador) no fechamento
        [Required]
        public decimal FundoDeCaixa { get; set; } // "Troco Inicial/Fundo de CX"

        [MaxLength(500)]
        public string? Observacoes { get; set; } // Campo "Observações"

        // Campos preenchidos (ou editados) APENAS pelo "Supervisor"
        public decimal? ValorAtm { get; set; }
        public decimal? ValorBoletosMensalistas { get; set; }

        
        // --- Propriedade de Navegação ---
        // Um FaturamentoDiario (dia) possui muitos FaturamentosParciais (turnos/lançamentos)
        public virtual ICollection<FaturamentoParcial> FaturamentosParciais { get; set; } = new List<FaturamentoParcial>();
    }
}