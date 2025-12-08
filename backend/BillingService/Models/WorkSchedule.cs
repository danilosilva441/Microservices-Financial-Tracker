using BillingService.Enums;
using SharedKernel.Entities;

namespace BillingService.Models
{
    public class WorkSchedule : BaseEntity, ITenantEntity
    {
        public Guid UnidadeId { get; set; } // Vinculado à unidade
        
        public string Name { get; set; } = string.Empty; // Ex: "Turno Manhã 12x36"
        public ScheduleType Type { get; set; }
        
        // Padrões para facilitar a criação dos dias
        public TimeSpan DefaultStartTime { get; set; } 
        public TimeSpan DefaultEndTime { get; set; }
        public int DefaultBreakDurationMinutes { get; set; }
        
        // Relacionamento EF
        public virtual Unidade? Unidade { get; set; }
    }
}