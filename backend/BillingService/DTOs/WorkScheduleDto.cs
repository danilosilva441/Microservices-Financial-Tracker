using BillingService.Enums;

namespace BillingService.DTOs.Shifts
{
    public class WorkScheduleDto
    {
        public Guid Id { get; set; }
        public Guid UnidadeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public ScheduleType Type { get; set; }
        
        // Formato "HH:mm:ss" para TimeSpans no JSON
        public string DefaultStartTime { get; set; } = "08:00:00";
        public string DefaultEndTime { get; set; } = "18:00:00";
        public int DefaultBreakDurationMinutes { get; set; }
    }
    
    public class CreateWorkScheduleDto
    {
        public Guid UnidadeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public ScheduleType Type { get; set; }
        public TimeSpan DefaultStartTime { get; set; }
        public TimeSpan DefaultEndTime { get; set; }
    }
}