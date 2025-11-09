// Caminho: backend/BillingService/Models/FaturamentoDiario.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BillingService.Models; // 1. MUDANÇA: Usando o namespace local

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
    
    // 2. MUDANÇA: Herda do BaseEntity local (para ganhar Id e TenantId)
    public class FaturamentoDiario : BaseEntity 
    {
        // 3. MUDANÇA: Link para a "Unidade" (novo nome)
        [Required]
        public Guid UnidadeId { get; set; } 
        
        [ForeignKey("UnidadeId")]
        public virtual Unidade Unidade { get; set; } = null!; // 4. MUDANÇA: Link para a classe Unidade

        [Required]
        public DateOnly Data { get; set; } // A data consolidada

        // 5. MUDANÇA: Usa o Enum para Status (mais seguro)
        [Required]
        public RegistroStatus Status { get; set; } = RegistroStatus.Pendente;

        
        // --- 6. NOVOS CAMPOS (Baseado nos formulários de papel / Roadmap 4.1) ---

        // Campos preenchidos pelo "Líder" (Operador) no fechamento
        [Required]
        public decimal FundoDeCaixa { get; set; } // "Troco Inicial/Fundo de CX"

        [MaxLength(500)]
        public string? Observacoes { get; set; } // Campo "Observações"

        // Campos preenchidos (ou editados) APENAS pelo "Supervisor"
        public decimal? ValorAtm { get; set; }
        public decimal? ValorBoletosMensalistas { get; set; }

        
        // --- 7. REMOVIDO (Importante) ---
        // public decimal ValorTotalConsolidado { get; set; }
        // POR QUÊ? O valor total será a SOMA dos FaturamentosParciais.
        // O FaturamentoDiario é o "cabeçalho" (a capa do formulário).
        // Os FaturamentosParciais são os "itens" (PIX, Dinheiro, etc.).
        // Vamos calcular o total no serviço, não armazená-lo aqui.

        
        // --- Propriedade de Navegação ---
        // Um FaturamentoDiario (dia) possui muitos FaturamentosParciais (turnos/lançamentos)
        public virtual ICollection<FaturamentoParcial> FaturamentosParciais { get; set; } = new List<FaturamentoParcial>();
    }
}