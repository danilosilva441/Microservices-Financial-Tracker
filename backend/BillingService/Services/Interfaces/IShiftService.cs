using BillingService.DTOs.Shifts;
using SharedKernel; // Para o objeto Result (sucesso/falha)
using SharedKernel.Common;

namespace BillingService.Services.Interfaces
{
    public interface IShiftService
    {
        // Cria um modelo (ex: "Turno Manhã")
        Task<Result<Guid>> CreateWorkScheduleAsync(CreateWorkScheduleDto dto, Guid tenantId);
        
        // A MÁGICA: Gera turnos automáticos (ex: projeta 12x36 para o mês todo)
        Task<Result> GenerateShiftsAsync(Guid unidadeId, Guid scheduleTemplateId, DateOnly startDate, DateOnly endDate, List<Guid> userIds, Guid tenantId);
        
        // Adiciona uma pausa (Almoço) validando horário
        Task<Result> AddBreakToShiftAsync(Guid shiftId, AddBreakDto dto, Guid tenantId);
        
        // Busca a escala para o calendário visual
        Task<Result<IEnumerable<EmployeeShiftDto>>> GetShiftsAsync(Guid unidadeId, DateOnly start, DateOnly end, Guid tenantId);
    }
}