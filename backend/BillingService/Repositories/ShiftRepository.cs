using BillingService.Data;
using BillingService.Models;
using BillingService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Common;
using SharedKernel.Data;

namespace BillingService.Repositories
{


    public class ShiftRepository : Repository<EmployeeShift>, IShiftRepository
    {

        private BillingDbContext MyContext => (BillingDbContext)_context;
        public ShiftRepository(BillingDbContext context) : base(context) { }

        public async Task<IEnumerable<EmployeeShift>> GetShiftsByPeriodAsync(Guid unidadeId, DateOnly startDate, DateOnly endDate, Guid tenantId)
        {
            return await MyContext.EmployeeShifts
                .Include(s => s.Breaks) // Traz as pausas junto
                .Where(s => s.UnidadeId == unidadeId &&
                            s.TenantId == tenantId &&
                            s.Date >= startDate &&
                            s.Date <= endDate)
                .OrderBy(s => s.Date)
                .ThenBy(s => s.ScheduledStartTime)
                .ToListAsync();
        }

        public async Task<bool> HasConflictAsync(Guid userId, DateTime start, DateTime end, Guid tenantId, Guid? ignoreShiftId = null)
        {
            // Lógica de Sobreposição de Horário:
            // (NovoInicio < FimExistente) E (NovoFim > InicioExistente)

            var query = MyContext.EmployeeShifts
                .Where(s => s.UserId == userId && s.TenantId == tenantId)
                .Where(s => start < s.ScheduledEndTime && end > s.ScheduledStartTime);

            if (ignoreShiftId.HasValue)
            {
                query = query.Where(s => s.Id != ignoreShiftId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<IEnumerable<WorkSchedule>> GetSchedulesByUnidadeAsync(Guid unidadeId, Guid tenantId)
        {
            return await MyContext.WorkSchedules
                .Where(w => w.UnidadeId == unidadeId && w.TenantId == tenantId)
                .ToListAsync();
        }

        public async Task AddScheduleAsync(WorkSchedule schedule)
        {
            await MyContext.WorkSchedules.AddAsync(schedule);
        }

        internal static async Task<EmployeeShift> GetByIdAsync(object id)
        {
            throw new NotImplementedException();
        }
    }
}