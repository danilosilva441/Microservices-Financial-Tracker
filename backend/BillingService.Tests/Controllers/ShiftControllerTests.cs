using Xunit;
using Moq;
using FluentAssertions;
using BillingService.Controllers;
using BillingService.Services.Interfaces;
using BillingService.DTOs.Shifts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using SharedKernel.Common;

namespace BillingService.Tests.Controllers
{
    public class ShiftControllerTests
    {
        private readonly Mock<IShiftService> _serviceMock;
        private readonly ShiftsController _controller;
        private readonly Guid _tenantId;
        private readonly ClaimsPrincipal _user;

        public ShiftControllerTests()
        {
            _serviceMock = new Mock<IShiftService>();
            _tenantId = Guid.NewGuid();
            
            _user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("tenantId", _tenantId.ToString()),
                new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
                new Claim(ClaimTypes.Name, "Test User")
            }, "TestAuthentication"));

            _controller = new ShiftsController(_serviceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = _user }
                }
            };
        }

        [Fact]
        public async Task CreateTemplate_ShouldReturnCreated_WhenServiceReturnsSuccess()
        {
            // Arrange
            var dto = new CreateWorkScheduleDto
            {
                Name = "Test Schedule",
                UnidadeId = Guid.NewGuid(),
            };
            
            var createdId = Guid.NewGuid();
            var serviceResult = Result.Ok(createdId);

            _serviceMock
                .Setup(s => s.CreateWorkScheduleAsync(dto, _tenantId))
                .ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.CreateTemplate(dto);

            // Assert
            var createdResult = result.Should().BeOfType<CreatedResult>().Subject;
            createdResult.StatusCode.Should().Be(StatusCodes.Status201Created);
            createdResult.Value.Should().Be(createdId);
            
            _serviceMock.Verify(
                s => s.CreateWorkScheduleAsync(dto, _tenantId),
                Times.Once);
        }

        [Fact]
        public async Task CreateTemplate_ShouldReturnBadRequest_WhenServiceReturnsFailure()
        {
            // Arrange
            var dto = new CreateWorkScheduleDto
            {
                Name = "Test Schedule",
                UnidadeId = Guid.NewGuid()
            };
            
            var errorMessage = "Erro de validação";
            var serviceResult = Result.Fail<Guid>(errorMessage);

            _serviceMock
                .Setup(s => s.CreateWorkScheduleAsync(dto, _tenantId))
                .ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.CreateTemplate(dto);

            // Assert
            var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequest.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            badRequest.Value.Should().Be(errorMessage);
        }

        [Fact]
        public async Task GetShifts_ShouldReturnOk_WhenServiceReturnsData()
        {
            // Arrange
            var unidadeId = Guid.NewGuid();
            var start = DateOnly.FromDateTime(DateTime.Now);
            var end = start.AddDays(7);
            
            var shifts = new List<EmployeeShiftDto>
            {
                new EmployeeShiftDto { 
                    Id = Guid.NewGuid()
                },
                new EmployeeShiftDto { 
                    Id = Guid.NewGuid()
                }
            };

            var serviceResult = Result.Ok<IEnumerable<EmployeeShiftDto>>(shifts);

            _serviceMock
                .Setup(s => s.GetShiftsAsync(unidadeId, start, end, _tenantId))
                .ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.GetShifts(unidadeId, start, end);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            okResult.Value.Should().BeSameAs(shifts);
            
            _serviceMock.Verify(
                s => s.GetShiftsAsync(unidadeId, start, end, _tenantId),
                Times.Once);
        }

        [Fact]
        public async Task GetShifts_ShouldReturnOk_WhenServiceReturnsEmptyList()
        {
            // Arrange
            var unidadeId = Guid.NewGuid();
            var start = DateOnly.FromDateTime(DateTime.Now);
            var end = start.AddDays(7);
            
            var emptyShifts = Enumerable.Empty<EmployeeShiftDto>();
            var serviceResult = Result.Ok<IEnumerable<EmployeeShiftDto>>(emptyShifts);

            _serviceMock
                .Setup(s => s.GetShiftsAsync(unidadeId, start, end, _tenantId))
                .ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.GetShifts(unidadeId, start, end);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            okResult.Value.Should().BeSameAs(emptyShifts);
        }

        [Fact]
        public async Task AddBreak_ShouldReturnOk_WhenServiceReturnsSuccess()
        {
            // Arrange
            var shiftId = Guid.NewGuid();
            var dto = new AddBreakDto();
            
            var serviceResult = Result.Ok();

            _serviceMock
                .Setup(s => s.AddBreakToShiftAsync(shiftId, dto, _tenantId))
                .ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.AddBreak(shiftId, dto);

            // Assert
            result.Should().BeOfType<OkResult>();
            
            _serviceMock.Verify(
                s => s.AddBreakToShiftAsync(shiftId, dto, _tenantId),
                Times.Once);
        }

        [Fact]
        public async Task AddBreak_ShouldReturnBadRequest_WhenServiceReturnsFailure()
        {
            // Arrange
            var shiftId = Guid.NewGuid();
            var dto = new AddBreakDto();
            
            var errorMessage = "Shift não encontrado";
            var serviceResult = Result.Fail(errorMessage);

            _serviceMock
                .Setup(s => s.AddBreakToShiftAsync(shiftId, dto, _tenantId))
                .ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.AddBreak(shiftId, dto);

            // Assert
            var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequest.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            badRequest.Value.Should().Be(errorMessage);
        }

        [Fact]
        public async Task CreateTemplate_ShouldThrowException_WhenTenantIdIsMissing()
        {
            // Arrange
            var controllerWithoutUser = new ShiftsController(_serviceMock.Object);
            
            var dto = new CreateWorkScheduleDto();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NullReferenceException>(() => 
                controllerWithoutUser.CreateTemplate(dto));
            
            exception.Message.Should().Contain("Object reference not set");
        }

        [Fact]
        public async Task GetShifts_ShouldThrowException_WhenTenantIdIsMissing()
        {
            // Arrange
            var controllerWithoutUser = new ShiftsController(_serviceMock.Object);
            
            var unidadeId = Guid.NewGuid();
            var start = DateOnly.FromDateTime(DateTime.Now);
            var end = start.AddDays(7);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NullReferenceException>(() => 
                controllerWithoutUser.GetShifts(unidadeId, start, end));
            
            exception.Message.Should().Contain("Object reference not set");
        }

        [Fact]
        public async Task AddBreak_ShouldThrowException_WhenTenantIdIsMissing()
        {
            // Arrange
            var controllerWithoutUser = new ShiftsController(_serviceMock.Object);
            
            var shiftId = Guid.NewGuid();
            var dto = new AddBreakDto();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NullReferenceException>(() => 
                controllerWithoutUser.AddBreak(shiftId, dto));
            
            exception.Message.Should().Contain("Object reference not set");
        }

        // Testes simplificados - usando apenas propriedades reais

        [Fact]
        public async Task CreateTemplate_ShouldCallServiceWithCorrectParameters()
        {
            // Arrange
            var dto = new CreateWorkScheduleDto
            {
                Name = "Test Schedule",
                UnidadeId = Guid.NewGuid()
                // Use apenas propriedades que realmente existem
            };
            
            var createdId = Guid.NewGuid();
            var serviceResult = Result.Ok(createdId);

            _serviceMock
                .Setup(s => s.CreateWorkScheduleAsync(It.Is<CreateWorkScheduleDto>(x => 
                    x.Name == dto.Name && 
                    x.UnidadeId == dto.UnidadeId), _tenantId))
                .ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.CreateTemplate(dto);

            // Assert
            result.Should().BeOfType<CreatedResult>();
            
            _serviceMock.Verify(
                s => s.CreateWorkScheduleAsync(
                    It.Is<CreateWorkScheduleDto>(x => x.Name == dto.Name),
                    _tenantId),
                Times.Once);
        }

        [Fact]
        public async Task GetShifts_ShouldCallServiceWithCorrectDateRange()
        {
            // Arrange
            var unidadeId = Guid.NewGuid();
            var start = DateOnly.FromDateTime(new DateTime(2024, 1, 1));
            var end = DateOnly.FromDateTime(new DateTime(2024, 1, 31));
            
            var shifts = new List<EmployeeShiftDto>();
            var serviceResult = Result.Ok<IEnumerable<EmployeeShiftDto>>(shifts);

            _serviceMock
                .Setup(s => s.GetShiftsAsync(unidadeId, start, end, _tenantId))
                .ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.GetShifts(unidadeId, start, end);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            
            _serviceMock.Verify(
                s => s.GetShiftsAsync(
                    unidadeId, 
                    It.Is<DateOnly>(d => d == start), 
                    It.Is<DateOnly>(d => d == end), 
                    _tenantId),
                Times.Once);
        }

        [Fact]
        public async Task AddBreak_ShouldCallServiceWithCorrectShiftId()
        {
            // Arrange
            var shiftId = Guid.NewGuid();
            var dto = new AddBreakDto();
            
            var serviceResult = Result.Ok();

            _serviceMock
                .Setup(s => s.AddBreakToShiftAsync(shiftId, dto, _tenantId))
                .ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.AddBreak(shiftId, dto);

            // Assert
            result.Should().BeOfType<OkResult>();
            
            _serviceMock.Verify(
                s => s.AddBreakToShiftAsync(
                    It.Is<Guid>(id => id == shiftId),
                    It.IsAny<AddBreakDto>(),
                    _tenantId),
                Times.Once);
        }

        [Fact]
        public async Task GetShifts_ShouldReturnOk_WhenDatesAreValid()
        {
            // Arrange
            var unidadeId = Guid.NewGuid();
            var start = DateOnly.FromDateTime(DateTime.Now);
            var end = start.AddDays(1); // Apenas 1 dia de diferença
            
            var shifts = new List<EmployeeShiftDto>();
            var serviceResult = Result.Ok<IEnumerable<EmployeeShiftDto>>(shifts);

            _serviceMock
                .Setup(s => s.GetShiftsAsync(unidadeId, start, end, _tenantId))
                .ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.GetShifts(unidadeId, start, end);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task CreateTemplate_ShouldHandleEmptyDto()
        {
            // Arrange
            var dto = new CreateWorkScheduleDto(); // DTO vazio
            
            var errorMessage = "Nome é obrigatório";
            var serviceResult = Result.Fail<Guid>(errorMessage);

            _serviceMock
                .Setup(s => s.CreateWorkScheduleAsync(dto, _tenantId))
                .ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.CreateTemplate(dto);

            // Assert
            var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequest.Value.Should().Be(errorMessage);
        }
    }
}