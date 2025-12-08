using BillingService.Enums;
using SharedKernel.Entities;

namespace BillingService.Models
{
    public class ShiftBreak : BaseEntity, ITenantEntity
    {
        public Guid EmployeeShiftId { get; set; }
        
        public BreakType Type { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        
        public bool IsPaid { get; set; } // Se conta como hora trabalhada

        public virtual EmployeeShift? EmployeeShift { get; set; }
    }
}