// backend/BillingService/Models/FaturamentoDiario.cs (ATUALIZE)

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BillingService.Models;
using SharedKernel.Entities;

namespace BillingService.Models
{
    public enum RegistroStatus
    {
        Pendente,   // Submetido pelo Líder/Operador
        Aprovado,   // Aprovado pelo Supervisor
        Rejeitado,  // Rejeitado pelo Supervisor
        Fechado,    // Caixa fechado pelo Operador
        Conferido   // Caixa conferido pelo Supervisor/Gerente
    }
    
    // Enum para Status do Caixa (específico para fechamento)
    public enum StatusCaixa
    {
        Aberto,
        Fechado,
        Conferido,
        Enviado
    }
    
    public class FaturamentoDiario : BaseEntity 
    {
        [Required]
        public Guid UnidadeId { get; set; } 
        
        [ForeignKey("UnidadeId")]
        public virtual Unidade Unidade { get; set; } = null!;

        [Required]
        public DateOnly Data { get; set; }

        [Required]
        public RegistroStatus Status { get; set; } = RegistroStatus.Pendente;

        // Status específico para controle de fechamento
        [Required]
        public StatusCaixa StatusCaixa { get; set; } = StatusCaixa.Aberto;
        
        // --- Campos preenchidos pelo "Líder" (Operador) ---
        [Required]
        public decimal FundoDeCaixa { get; set; }

        [MaxLength(500)]
        public string? Observacoes { get; set; }

        // --- Campos do Supervisor ---
        public decimal? ValorAtm { get; set; }
        public decimal? ValorBoletosMensalistas { get; set; }
        
        // --- NOVOS CAMPOS PARA FECHAMENTO ---
        public decimal? ValorTotalCalculado { get; set; } // Soma dos parciais
        public decimal? ValorConferido { get; set; } // Valor contado pelo operador
        public decimal? Diferenca { get; set; } // Diferença entre calculado e conferido
        
        [MaxLength(64)]
        public string? HashAssinatura { get; set; } // Hash de segurança
        
        public DateTime? DataFechamento { get; set; }
        public Guid? FechadoPorUserId { get; set; }
        
        public DateTime? DataConferencia { get; set; }
        public Guid? ConferidoPorUserId { get; set; }
        
        [MaxLength(500)]
        public string? ObservacoesConferencia { get; set; }
        
        // --- Propriedade de Navegação ---
        public virtual ICollection<FaturamentoParcial> FaturamentosParciais { get; set; } = new List<FaturamentoParcial>();
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorTotal { get; set; }
    }
}