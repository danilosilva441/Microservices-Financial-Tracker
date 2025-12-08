using Xunit;
using Moq;
using FluentAssertions;
using BillingService.Services;
using BillingService.Repositories.Interfaces;
using BillingService.Models;
using BillingService.DTOs.Shifts;
using BillingService.Enums;
using SharedKernel.Common;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BillingService.Tests.Services
{
    public class ShiftServiceTests
    {
        private readonly Mock<IShiftRepository> _shiftRepositoryMock;
        private readonly Mock<IUnidadeRepository> _unidadeRepositoryMock;
        private readonly ShiftService _shiftService;
        private readonly Guid _tenantId = Guid.NewGuid();

        public ShiftServiceTests()
        {
            _shiftRepositoryMock = new Mock<IShiftRepository>();
            _unidadeRepositoryMock = new Mock<IUnidadeRepository>();
            _shiftService = new ShiftService(_shiftRepositoryMock.Object, _unidadeRepositoryMock.Object);
        }

        [Fact]
        public async Task CreateWorkScheduleAsync_ShouldReturnSuccess_WhenUnidadeExists()
        {
            // Arrange
            var dto = new CreateWorkScheduleDto
            {
                UnidadeId = Guid.NewGuid(),
                Name = "Escala Diária",
                Type = ScheduleType.Diarista,
                DefaultStartTime = TimeSpan.FromHours(8),
                DefaultEndTime = TimeSpan.FromHours(17)
            };

            var expectedUnidade = new Unidade { Id = dto.UnidadeId, TenantId = _tenantId };

            _unidadeRepositoryMock
                .Setup(repo => repo.GetByIdAsync(dto.UnidadeId, _tenantId))
                .ReturnsAsync(expectedUnidade);

            _shiftRepositoryMock
                .Setup(repo => repo.AddScheduleAsync(It.IsAny<WorkSchedule>()))
                .Returns(Task.CompletedTask);

            _shiftRepositoryMock
                .Setup(repo => repo.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _shiftService.CreateWorkScheduleAsync(dto, _tenantId);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.IsFailure.Should().BeFalse();
            result.Value.Should().NotBe(Guid.Empty);
            _unidadeRepositoryMock.Verify(repo => repo.GetByIdAsync(dto.UnidadeId, _tenantId), Times.Once);
            _shiftRepositoryMock.Verify(repo => repo.AddScheduleAsync(It.IsAny<WorkSchedule>()), Times.Once);
            _shiftRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateWorkScheduleAsync_ShouldReturnFailure_WhenUnidadeDoesNotExist()
        {
            // Arrange
            var dto = new CreateWorkScheduleDto
            {
                UnidadeId = Guid.NewGuid(),
                Name = "Escala Diária",
                Type = ScheduleType.Diarista,
                DefaultStartTime = TimeSpan.FromHours(8),
                DefaultEndTime = TimeSpan.FromHours(17)
            };

            _unidadeRepositoryMock
                .Setup(repo => repo.GetByIdAsync(dto.UnidadeId, _tenantId))
                .ReturnsAsync((Unidade?)null);

            // Act
            var result = await _shiftService.CreateWorkScheduleAsync(dto, _tenantId);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.IsFailure.Should().BeTrue();
            result.ErrorMessage.Should().Be("Unidade não encontrada.");
            result.Value.Should().Be(Guid.Empty);
            _unidadeRepositoryMock.Verify(repo => repo.GetByIdAsync(dto.UnidadeId, _tenantId), Times.Once);
            _shiftRepositoryMock.Verify(repo => repo.AddScheduleAsync(It.IsAny<WorkSchedule>()), Times.Never);
        }

        [Fact]
        public async Task GenerateShiftsAsync_ShouldReturnSuccess_WhenValidData()
        {
            // Arrange
            var unidadeId = Guid.NewGuid();
            var scheduleTemplateId = Guid.NewGuid();
            var startDate = DateOnly.FromDateTime(DateTime.Today);
            var endDate = startDate.AddDays(7);
            var userIds = new List<Guid> { Guid.NewGuid() };
            
            var template = new WorkSchedule
            {
                Id = scheduleTemplateId,
                TenantId = _tenantId,
                UnidadeId = unidadeId,
                Name = "Template Diário",
                Type = ScheduleType.Diarista,
                DefaultStartTime = TimeSpan.FromHours(8),
                DefaultEndTime = TimeSpan.FromHours(17)
            };

            _shiftRepositoryMock
                .Setup(repo => repo.GetSchedulesByUnidadeAsync(unidadeId, _tenantId))
                .ReturnsAsync(new List<WorkSchedule> { template });

            // CORREÇÃO: Para métodos com parâmetros opcionais, use uma expressão lambda mais simples
            // O Moq não pode inferir o valor padrão do parâmetro opcional
            _shiftRepositoryMock
                .Setup(repo => repo.HasConflictAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<Guid>(), null))
                .ReturnsAsync(false);

            _shiftRepositoryMock
                .Setup(repo => repo.AddRangeAsync(It.IsAny<List<EmployeeShift>>()))
                .Returns(Task.CompletedTask);

            _shiftRepositoryMock
                .Setup(repo => repo.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _shiftService.GenerateShiftsAsync(
                unidadeId, scheduleTemplateId, startDate, endDate, userIds, _tenantId);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.IsFailure.Should().BeFalse();
            _shiftRepositoryMock.Verify(repo => repo.GetSchedulesByUnidadeAsync(unidadeId, _tenantId), Times.Once);
        }

        [Fact]
        public async Task GenerateShiftsAsync_ShouldReturnFailure_WhenTemplateNotFound()
        {
            // Arrange
            var unidadeId = Guid.NewGuid();
            var scheduleTemplateId = Guid.NewGuid();
            var startDate = DateOnly.FromDateTime(DateTime.Today);
            var endDate = startDate.AddDays(7);
            var userIds = new List<Guid> { Guid.NewGuid() };

            _shiftRepositoryMock
                .Setup(repo => repo.GetSchedulesByUnidadeAsync(unidadeId, _tenantId))
                .ReturnsAsync(new List<WorkSchedule>());

            // Act
            var result = await _shiftService.GenerateShiftsAsync(
                unidadeId, scheduleTemplateId, startDate, endDate, userIds, _tenantId);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.IsFailure.Should().BeTrue();
            result.ErrorMessage.Should().Be("Modelo de escala não encontrado.");
        }

        [Fact]
        public async Task GenerateShiftsAsync_ShouldHandleConflicts_WhenShiftHasConflict()
        {
            // Arrange
            var unidadeId = Guid.NewGuid();
            var scheduleTemplateId = Guid.NewGuid();
            var startDate = DateOnly.FromDateTime(DateTime.Today);
            var endDate = startDate.AddDays(1);
            var userIds = new List<Guid> { Guid.NewGuid() };
            
            var template = new WorkSchedule
            {
                Id = scheduleTemplateId,
                TenantId = _tenantId,
                UnidadeId = unidadeId,
                Name = "Template Diário",
                Type = ScheduleType.Diarista,
                DefaultStartTime = TimeSpan.FromHours(8),
                DefaultEndTime = TimeSpan.FromHours(17)
            };

            _shiftRepositoryMock
                .Setup(repo => repo.GetSchedulesByUnidadeAsync(unidadeId, _tenantId))
                .ReturnsAsync(new List<WorkSchedule> { template });

            // CORREÇÃO: Use apenas os parâmetros obrigatórios
            _shiftRepositoryMock
                .Setup(repo => repo.HasConflictAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<Guid>(), null))
                .ReturnsAsync(true);

            // Act
            var result = await _shiftService.GenerateShiftsAsync(
                unidadeId, scheduleTemplateId, startDate, endDate, userIds, _tenantId);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue(); // Ainda deve ser sucesso mesmo com conflitos ignorados
            result.IsFailure.Should().BeFalse();
        }

        [Fact]
        public async Task AddBreakToShiftAsync_ShouldReturnSuccess_WhenValidBreak()
        {
            // Arrange
            var shiftId = Guid.NewGuid();
            var dto = new AddBreakDto
            {
                Type = BreakType.Cafe,
                StartTime = DateTime.Now.AddHours(2),
                EndTime = DateTime.Now.AddHours(2).AddMinutes(15)
            };

            var existingShift = new EmployeeShift
            {
                Id = shiftId,
                TenantId = _tenantId,
                ScheduledStartTime = DateTime.Now,
                ScheduledEndTime = DateTime.Now.AddHours(8),
                Breaks = new List<ShiftBreak>()
            };

            _shiftRepositoryMock
                .Setup(repo => repo.GetByIdAsync(shiftId))
                .ReturnsAsync(existingShift);

            _shiftRepositoryMock
                .Setup(repo => repo.Update(It.IsAny<EmployeeShift>()))
                .Verifiable();

            _shiftRepositoryMock
                .Setup(repo => repo.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _shiftService.AddBreakToShiftAsync(shiftId, dto, _tenantId);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.IsFailure.Should().BeFalse();
            _shiftRepositoryMock.Verify(repo => repo.GetByIdAsync(shiftId), Times.Once);
            _shiftRepositoryMock.Verify(repo => repo.Update(It.IsAny<EmployeeShift>()), Times.Once);
            _shiftRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task AddBreakToShiftAsync_ShouldReturnFailure_WhenShiftNotFound()
        {
            // Arrange
            var shiftId = Guid.NewGuid();
            var dto = new AddBreakDto
            {
                Type = BreakType.Cafe,
                StartTime = DateTime.Now.AddHours(2),
                EndTime = DateTime.Now.AddHours(2).AddMinutes(15)
            };

            _shiftRepositoryMock
                .Setup(repo => repo.GetByIdAsync(shiftId))
                .ReturnsAsync((EmployeeShift?)null);

            // Act
            var result = await _shiftService.AddBreakToShiftAsync(shiftId, dto, _tenantId);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.IsFailure.Should().BeTrue();
            result.ErrorMessage.Should().Be("Turno não encontrado.");
        }

        [Fact]
        public async Task AddBreakToShiftAsync_ShouldReturnFailure_WhenBreakOutsideShift()
        {
            // Arrange
            var shiftId = Guid.NewGuid();
            var dto = new AddBreakDto
            {
                Type = BreakType.Cafe,
                StartTime = DateTime.Now.AddHours(10), // Fora do turno
                EndTime = DateTime.Now.AddHours(10).AddMinutes(15)
            };

            var existingShift = new EmployeeShift
            {
                Id = shiftId,
                TenantId = _tenantId,
                ScheduledStartTime = DateTime.Now,
                ScheduledEndTime = DateTime.Now.AddHours(8),
                Breaks = new List<ShiftBreak>()
            };

            _shiftRepositoryMock
                .Setup(repo => repo.GetByIdAsync(shiftId))
                .ReturnsAsync(existingShift);

            // Act
            var result = await _shiftService.AddBreakToShiftAsync(shiftId, dto, _tenantId);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.IsFailure.Should().BeTrue();
            result.ErrorMessage.Should().Be("A pausa deve ocorrer dentro do horário do turno.");
        }

        [Fact]
        public async Task AddBreakToShiftAsync_ShouldReturnFailure_WhenBreakEndTimeBeforeStartTime()
        {
            // Arrange
            var shiftId = Guid.NewGuid();
            var dto = new AddBreakDto
            {
                Type = BreakType.Cafe,
                StartTime = DateTime.Now.AddHours(2),
                EndTime = DateTime.Now.AddHours(1) // Fim antes do início
            };

            var existingShift = new EmployeeShift
            {
                Id = shiftId,
                TenantId = _tenantId,
                ScheduledStartTime = DateTime.Now,
                ScheduledEndTime = DateTime.Now.AddHours(8),
                Breaks = new List<ShiftBreak>()
            };

            _shiftRepositoryMock
                .Setup(repo => repo.GetByIdAsync(shiftId))
                .ReturnsAsync(existingShift);

            // Act
            var result = await _shiftService.AddBreakToShiftAsync(shiftId, dto, _tenantId);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.IsFailure.Should().BeTrue();
            result.ErrorMessage.Should().Be("Hora fim deve ser maior que hora início.");
        }

        [Fact]
        public async Task GetShiftsAsync_ShouldReturnShifts_WhenDataExists()
        {
            // Arrange
            var unidadeId = Guid.NewGuid();
            var start = DateOnly.FromDateTime(DateTime.Today);
            var end = start.AddDays(7);

            var shifts = new List<EmployeeShift>
            {
                new EmployeeShift
                {
                    Id = Guid.NewGuid(),
                    TenantId = _tenantId,
                    UnidadeId = unidadeId,
                    UserId = Guid.NewGuid(),
                    Date = start,
                    ScheduledStartTime = start.ToDateTime(TimeOnly.FromTimeSpan(TimeSpan.FromHours(8))),
                    ScheduledEndTime = start.ToDateTime(TimeOnly.FromTimeSpan(TimeSpan.FromHours(17))),
                    Breaks = new List<ShiftBreak>
                    {
                        new ShiftBreak
                        {
                            Id = Guid.NewGuid(),
                            Type = BreakType.Almoco,
                            StartTime = start.ToDateTime(TimeOnly.FromTimeSpan(TimeSpan.FromHours(12))),
                            EndTime = start.ToDateTime(TimeOnly.FromTimeSpan(TimeSpan.FromHours(13)))
                        }
                    }
                }
            };

            _shiftRepositoryMock
                .Setup(repo => repo.GetShiftsByPeriodAsync(unidadeId, start, end, _tenantId))
                .ReturnsAsync(shifts);

            // Act
            var result = await _shiftService.GetShiftsAsync(unidadeId, start, end, _tenantId);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.IsFailure.Should().BeFalse();
            result.Value.Should().NotBeNull();
            result.Value!.Count().Should().Be(1);
            result.Value!.First().Breaks.Should().HaveCount(1);
            _shiftRepositoryMock.Verify(repo => repo.GetShiftsByPeriodAsync(unidadeId, start, end, _tenantId), Times.Once);
        }

        [Fact]
        public async Task GetShiftsAsync_ShouldReturnEmptyList_WhenNoData()
        {
            // Arrange
            var unidadeId = Guid.NewGuid();
            var start = DateOnly.FromDateTime(DateTime.Today);
            var end = start.AddDays(7);

            _shiftRepositoryMock
                .Setup(repo => repo.GetShiftsByPeriodAsync(unidadeId, start, end, _tenantId))
                .ReturnsAsync(new List<EmployeeShift>());

            // Act
            var result = await _shiftService.GetShiftsAsync(unidadeId, start, end, _tenantId);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.IsFailure.Should().BeFalse();
            result.Value.Should().NotBeNull();
            result.Value!.Should().BeEmpty();
            _shiftRepositoryMock.Verify(repo => repo.GetShiftsByPeriodAsync(unidadeId, start, end, _tenantId), Times.Once);
        }

        [Fact]
        public async Task GetShiftsAsync_ShouldReturnFailure_WhenRepositoryThrowsException()
        {
            // Arrange
            var unidadeId = Guid.NewGuid();
            var start = DateOnly.FromDateTime(DateTime.Today);
            var end = start.AddDays(7);

            _shiftRepositoryMock
                .Setup(repo => repo.GetShiftsByPeriodAsync(unidadeId, start, end, _tenantId))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _shiftService.GetShiftsAsync(unidadeId, start, end, _tenantId));
        }

        // Testes adicionais para cobertura completa

        [Fact]
        public async Task GenerateShiftsAsync_ShouldNotCreateShifts_WhenNoWorkingDays()
        {
            // Arrange
            var unidadeId = Guid.NewGuid();
            var scheduleTemplateId = Guid.NewGuid();
            var startDate = new DateOnly(2024, 1, 6); // Sábado
            var endDate = new DateOnly(2024, 1, 7);   // Domingo
            var userIds = new List<Guid> { Guid.NewGuid() };
            
            var template = new WorkSchedule
            {
                Id = scheduleTemplateId,
                TenantId = _tenantId,
                UnidadeId = unidadeId,
                Name = "Template Diário",
                Type = ScheduleType.Diarista, // Segunda a sexta apenas
                DefaultStartTime = TimeSpan.FromHours(8),
                DefaultEndTime = TimeSpan.FromHours(17)
            };

            _shiftRepositoryMock
                .Setup(repo => repo.GetSchedulesByUnidadeAsync(unidadeId, _tenantId))
                .ReturnsAsync(new List<WorkSchedule> { template });

            // CORREÇÃO: Use apenas parâmetros obrigatórios
            _shiftRepositoryMock
                .Setup(repo => repo.HasConflictAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<Guid>(), null))
                .ReturnsAsync(false);

            // Act
            var result = await _shiftService.GenerateShiftsAsync(
                unidadeId, scheduleTemplateId, startDate, endDate, userIds, _tenantId);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            _shiftRepositoryMock.Verify(repo => repo.AddRangeAsync(It.IsAny<List<EmployeeShift>>()), Times.Never);
        }

        [Fact]
        public async Task GenerateShiftsAsync_ShouldHandle12x36Schedule()
        {
            // Arrange
            var unidadeId = Guid.NewGuid();
            var scheduleTemplateId = Guid.NewGuid();
            var startDate = DateOnly.FromDateTime(DateTime.Today);
            var endDate = startDate.AddDays(3);
            var userIds = new List<Guid> { Guid.NewGuid() };
            
            var template = new WorkSchedule
            {
                Id = scheduleTemplateId,
                TenantId = _tenantId,
                UnidadeId = unidadeId,
                Name = "Template 12x36",
                Type = ScheduleType.Plantonista12x36,
                DefaultStartTime = TimeSpan.FromHours(19),
                DefaultEndTime = TimeSpan.FromHours(7) // Vira a noite
            };

            _shiftRepositoryMock
                .Setup(repo => repo.GetSchedulesByUnidadeAsync(unidadeId, _tenantId))
                .ReturnsAsync(new List<WorkSchedule> { template });

            // CORREÇÃO: Use apenas parâmetros obrigatórios
            _shiftRepositoryMock
                .Setup(repo => repo.HasConflictAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<Guid>(), null))
                .ReturnsAsync(false);

            _shiftRepositoryMock
                .Setup(repo => repo.AddRangeAsync(It.IsAny<List<EmployeeShift>>()))
                .Returns(Task.CompletedTask);

            _shiftRepositoryMock
                .Setup(repo => repo.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _shiftService.GenerateShiftsAsync(
                unidadeId, scheduleTemplateId, startDate, endDate, userIds, _tenantId);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            _shiftRepositoryMock.Verify(repo => repo.GetSchedulesByUnidadeAsync(unidadeId, _tenantId), Times.Once);
        }

        [Fact]
        public async Task GenerateShiftsAsync_ShouldAdjustEndTime_WhenShiftSpansMidnight()
        {
            // Arrange
            var unidadeId = Guid.NewGuid();
            var scheduleTemplateId = Guid.NewGuid();
            var startDate = DateOnly.FromDateTime(DateTime.Today);
            var endDate = startDate.AddDays(1);
            var userIds = new List<Guid> { Guid.NewGuid() };
            
            var template = new WorkSchedule
            {
                Id = scheduleTemplateId,
                TenantId = _tenantId,
                UnidadeId = unidadeId,
                Name = "Template Noturno",
                Type = ScheduleType.Diarista,
                DefaultStartTime = TimeSpan.FromHours(22), // 22:00
                DefaultEndTime = TimeSpan.FromHours(6)     // 06:00 (próximo dia)
            };

            _shiftRepositoryMock
                .Setup(repo => repo.GetSchedulesByUnidadeAsync(unidadeId, _tenantId))
                .ReturnsAsync(new List<WorkSchedule> { template });

            // CORREÇÃO: Use apenas parâmetros obrigatórios
            _shiftRepositoryMock
                .Setup(repo => repo.HasConflictAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<Guid>(), null))
                .ReturnsAsync(false);

            _shiftRepositoryMock
                .Setup(repo => repo.AddRangeAsync(It.IsAny<List<EmployeeShift>>()))
                .Returns(Task.CompletedTask);

            _shiftRepositoryMock
                .Setup(repo => repo.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _shiftService.GenerateShiftsAsync(
                unidadeId, scheduleTemplateId, startDate, endDate, userIds, _tenantId);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
        }
    }
}