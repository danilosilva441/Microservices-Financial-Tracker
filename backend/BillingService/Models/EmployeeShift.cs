using SharedKernel.Entities;
using SharedKernel;
using BillingService.Enums;


namespace BillingService.Models
{
    public class EmployeeShift : BaseEntity, ITenantEntity
    {
        public Guid UnidadeId { get; set; }
        public Guid UserId { get; set; } // O Funcionário (Líder/Operador)
        
        public DateOnly Date { get; set; } // A data do plantão
        
        // Horário Planejado (Escala)
        public DateTime ScheduledStartTime { get; set; }
        public DateTime ScheduledEndTime { get; set; }
        
        // Horário Realizado (Ponto Eletrônico - Futuro)
        public DateTime? ActualStartTime { get; set; }
        public DateTime? ActualEndTime { get; set; }

        public string? Notes { get; set; } // Ex: "Trocou com Fulano"

        // Lista de Pausas (Almoço, Descanso)
        public virtual ICollection<ShiftBreak> Breaks { get; set; } = new List<ShiftBreak>();
        
        public virtual Unidade? Unidade { get; set; }
    }
}