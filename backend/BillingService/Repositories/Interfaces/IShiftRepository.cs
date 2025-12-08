using BillingService.Models;
using SharedKernel.Data;

namespace BillingService.Repositories.Interfaces
{
    public interface IShiftRepository : IRepository<EmployeeShift>
    {
        // Busca escalas de um período (ex: Mês atual) para exibir no calendário
        Task<IEnumerable<EmployeeShift>> GetShiftsByPeriodAsync(Guid unidadeId, DateOnly startDate, DateOnly endDate, Guid tenantId);
        
        // Verifica se já existe turno nesse horário para esse funcionário
        Task<bool> HasConflictAsync(Guid userId, DateTime start, DateTime end, Guid tenantId, Guid? ignoreShiftId = null);
        
        // Métodos para WorkSchedule (Modelos)
        Task<IEnumerable<WorkSchedule>> GetSchedulesByUnidadeAsync(Guid unidadeId, Guid tenantId);
        Task AddScheduleAsync(WorkSchedule schedule);
    }
}