using BillingService.DTOs.Shifts;
using BillingService.Enums;
using BillingService.Models;
using BillingService.Repositories.Interfaces;
using BillingService.Services.Interfaces;
using SharedKernel.Common;
using SharedKernel.Data;

namespace BillingService.Services
{
    public class ShiftService : IShiftService
    {
        private readonly IShiftRepository _shiftRepo;
        private readonly IUnidadeRepository _unidadeRepo; // Para validar se a unidade existe
        private readonly IShiftRepository _shiftRepository;    
        public ShiftService(IShiftRepository shiftRepo, IUnidadeRepository unidadeRepo)
        {
            _shiftRepo = shiftRepo;
            _unidadeRepo = unidadeRepo;
            _shiftRepository = shiftRepo;
        }

        public async Task<Result<Guid>> CreateWorkScheduleAsync(CreateWorkScheduleDto dto, Guid tenantId)
        {
            // Valida Unidade
            var unidade = await _unidadeRepo.GetByIdAsync(dto.UnidadeId, tenantId);
            if (unidade == null) return Result.Fail<Guid>("Unidade não encontrada.");

            var schedule = new WorkSchedule
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                UnidadeId = dto.UnidadeId,
                Name = dto.Name,
                Type = dto.Type,
                DefaultStartTime = dto.DefaultStartTime,
                DefaultEndTime = dto.DefaultEndTime
            };

            await _shiftRepo.AddScheduleAsync(schedule);
            await _shiftRepo.SaveChangesAsync();

            return Result.Ok(schedule.Id);
        }

        public async Task<Result> GenerateShiftsAsync(Guid unidadeId, Guid scheduleTemplateId, DateOnly startDate, DateOnly endDate, List<Guid> userIds, Guid tenantId)
        {
            // 1. Buscar o Modelo (Template)
            var schedules = await _shiftRepo.GetSchedulesByUnidadeAsync(unidadeId, tenantId);
            var template = schedules.FirstOrDefault(s => s.Id == scheduleTemplateId);
            
            if (template == null) return Result.Fail("Modelo de escala não encontrado.");

            var shiftsToAdd = new List<EmployeeShift>();
            var errors = new List<string>();

            // 2. Iterar sobre cada funcionário
            foreach (var userId in userIds)
            {
                // Lógica de Geração baseada no Tipo
                DateOnly currentDate = startDate;
                bool isWorkingDay = true; // Para 12x36, alterna true/false

                while (currentDate <= endDate)
                {
                    bool shouldCreateShift = false;

                    switch (template.Type)
                    {
                        case ScheduleType.Diarista:
                            // Segunda a Sexta (ignora Sábado e Domingo)
                            if (currentDate.DayOfWeek != DayOfWeek.Saturday && currentDate.DayOfWeek != DayOfWeek.Sunday)
                                shouldCreateShift = true;
                            break;

                        case ScheduleType.Plantonista12x36:
                            // Um dia sim, um dia não
                            if (isWorkingDay) shouldCreateShift = true;
                            isWorkingDay = !isWorkingDay; // Inverte para o próximo dia
                            break;
                            
                        // Outros tipos podem ser adicionados aqui
                    }

                    if (shouldCreateShift)
                    {
                        // Converter DateOnly + TimeSpan para DateTime
                        var startDateTime = currentDate.ToDateTime(TimeOnly.FromTimeSpan(template.DefaultStartTime));
                        var endDateTime = currentDate.ToDateTime(TimeOnly.FromTimeSpan(template.DefaultEndTime));

                        // Se virar a noite (ex: 22:00 as 06:00), adiciona 1 dia no fim
                        if (endDateTime < startDateTime) endDateTime = endDateTime.AddDays(1);

                        // 3. Validação de Conflito (A Regra de Ouro)
                        bool hasConflict = await _shiftRepo.HasConflictAsync(userId, startDateTime, endDateTime, tenantId);
                        if (!hasConflict)
                        {
                            shiftsToAdd.Add(new EmployeeShift
                            {
                                Id = Guid.NewGuid(),
                                TenantId = tenantId,
                                UnidadeId = unidadeId,
                                UserId = userId,
                                Date = currentDate,
                                ScheduledStartTime = startDateTime,
                                ScheduledEndTime = endDateTime
                            });
                        }
                        else
                        {
                            errors.Add($"Conflito ignorado para usuário {userId} em {currentDate}.");
                        }
                    }

                    currentDate = currentDate.AddDays(1);
                }
            }

            if (shiftsToAdd.Any())
            {
                await _shiftRepo.AddRangeAsync(shiftsToAdd); // Assumindo que seu Repo genérico tem AddRange
                await _shiftRepo.SaveChangesAsync();
            }

            return Result.Ok();
        }

        public async Task<Result> AddBreakToShiftAsync(Guid shiftId, AddBreakDto dto, Guid tenantId)
        {
            var shift = await _shiftRepository.GetByIdAsync(shiftId);
            if (shift == null) return Result.Fail("Turno não encontrado.");

            // Validação: A pausa deve estar DENTRO do turno
            if (dto.StartTime < shift.ScheduledStartTime || dto.EndTime > shift.ScheduledEndTime)
            {
                return Result.Fail("A pausa deve ocorrer dentro do horário do turno.");
            }

            if (dto.EndTime <= dto.StartTime)
            {
                return Result.Fail("Hora fim deve ser maior que hora início.");
            }

            var newBreak = new ShiftBreak
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                EmployeeShiftId = shiftId,
                Type = dto.Type,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                IsPaid = dto.Type != BreakType.Almoco // Exemplo: Almoço não paga, Café paga
            };

            // Aqui você poderia adicionar: _context.ShiftBreaks.Add(newBreak); 
            // Mas como estamos no repo genérico, vamos assumir que precisamos de um método específico ou acesso ao contexto.
            // Para simplificar, vou adicionar à coleção do objeto e dar Update no pai.
            shift.Breaks.Add(newBreak);
            
            _shiftRepo.Update(shift);
            await _shiftRepo.SaveChangesAsync();

            return Result.Ok();
        }

        public async Task<Result<IEnumerable<EmployeeShiftDto>>> GetShiftsAsync(Guid unidadeId, DateOnly start, DateOnly end, Guid tenantId)
        {
            var shifts = await _shiftRepo.GetShiftsByPeriodAsync(unidadeId, start, end, tenantId);

            // Mapeamento Manual (para simplificar sem AutoMapper)
            var dtos = shifts.Select(s => new EmployeeShiftDto
            {
                Id = s.Id,
                UserId = s.UserId,
                Date = s.Date,
                ScheduledStartTime = s.ScheduledStartTime,
                ScheduledEndTime = s.ScheduledEndTime,
                Notes = s.Notes,
                Breaks = s.Breaks.Select(b => new ShiftBreakDto
                {
                    Id = b.Id,
                    Type = b.Type,
                    StartTime = b.StartTime,
                    EndTime = b.EndTime
                }).ToList()
            });

            return Result.Ok(dtos);
        }
    }
}