using BillingService.Enums;

namespace BillingService.DTOs.Shifts
{
    public class EmployeeShiftDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty; // Para facilitar o Frontend
        public DateOnly Date { get; set; }
        
        public DateTime ScheduledStartTime { get; set; }
        public DateTime ScheduledEndTime { get; set; }
        public string? Notes { get; set; }
        
        public List<ShiftBreakDto> Breaks { get; set; } = new();
    }

    public class CreateShiftDto
    {
        public Guid UnidadeId { get; set; }
        public Guid UserId { get; set; }
        public DateOnly Date { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Notes { get; set; }
    }
    
    public class AddBreakDto
    {
        public BreakType Type { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }

    public class ShiftBreakDto : AddBreakDto 
    {
        public Guid Id { get; set; }
    }
}