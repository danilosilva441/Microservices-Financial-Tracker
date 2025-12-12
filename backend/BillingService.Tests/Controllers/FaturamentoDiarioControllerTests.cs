using BillingService.Controllers;
using BillingService.DTOs;
using BillingService.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SharedKernel;
using System.Security.Claims;
using Xunit;

namespace BillingService.Tests.Controllers
{
    public class FaturamentoDiarioControllerTests
    {
        private readonly Mock<IFaturamentoDiarioService> _serviceMock;
        private readonly Mock<ILogger<FaturamentoDiarioController>> _loggerMock;
        private readonly FaturamentoDiarioController _controller;
        private readonly Guid _userId;
        private readonly Guid _tenantId;

        public FaturamentoDiarioControllerTests()
        {
            _serviceMock = new Mock<IFaturamentoDiarioService>();
            _loggerMock = new Mock<ILogger<FaturamentoDiarioController>>();
            
            // Gerar IDs uma vez para reutilizar nos testes
            _userId = Guid.NewGuid();
            _tenantId = Guid.NewGuid();
            
            _controller = new FaturamentoDiarioController(_serviceMock.Object, _loggerMock.Object);
            
            ConfigurarContextoUsuario(_userId, _tenantId);
        }

        [Fact]
        public async Task SubmeterFechamento_Sucesso_DeveRetornarCreated()
        {
            // Arrange
            var unidadeId = Guid.NewGuid();
            var dto = new FaturamentoDiarioCreateDto 
            { 
                Data = DateOnly.FromDateTime(DateTime.Now.AddDays(-1))
            };
            var responseDto = new FaturamentoDiarioResponseDto 
            { 
                Id = Guid.NewGuid(),
                Data = dto.Data,
                Status = "Pendente",
                UnidadeId = unidadeId
            };

            _serviceMock
                .Setup(s => s.SubmeterFechamentoAsync(unidadeId, dto, _userId, _tenantId))
                .ReturnsAsync((responseDto, null));

            // Act
            var result = await _controller.SubmeterFechamento(unidadeId, dto);

            // Assert
            var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.StatusCode.Should().Be(201);
            createdResult.Value.Should().Be(responseDto);
            createdResult.ActionName.Should().Be(nameof(_controller.GetFechamentoById));
            
            _serviceMock.Verify(
                s => s.SubmeterFechamentoAsync(unidadeId, dto, _userId, _tenantId),
                Times.Once);
        }

        [Fact]
        public async Task SubmeterFechamento_Duplicado_DeveRetornarConflict()
        {
            // Arrange
            var unidadeId = Guid.NewGuid();
            var dto = new FaturamentoDiarioCreateDto 
            { 
                Data = DateOnly.FromDateTime(DateTime.Now) 
            };
            
            _serviceMock
                .Setup(s => s.SubmeterFechamentoAsync(unidadeId, dto, It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync((null, ErrorMessages.FechamentoJaExiste));

            // Act
            var result = await _controller.SubmeterFechamento(unidadeId, dto);

            // Assert
            var conflictResult = result.Should().BeOfType<ConflictObjectResult>().Subject;
            conflictResult.StatusCode.Should().Be(409);
            conflictResult.Value.Should().Be(ErrorMessages.FechamentoJaExiste);
        }

        [Fact]
        public async Task SubmeterFechamento_ErroValidacao_DeveRetornarBadRequest()
        {
            // Arrange
            var unidadeId = Guid.NewGuid();
            var dto = new FaturamentoDiarioCreateDto 
            { 
                Data = DateOnly.FromDateTime(DateTime.Now) 
            };
            var erroValidacao = "Data inválida para fechamento";
            
            _serviceMock
                .Setup(s => s.SubmeterFechamentoAsync(unidadeId, dto, It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync((null, erroValidacao));

            // Act
            var result = await _controller.SubmeterFechamento(unidadeId, dto);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be(erroValidacao);
        }

        [Fact]
        public async Task SubmeterFechamento_UsuarioNaoAutorizado_DeveRetornarUnauthorized()
        {
            // Arrange
            var unidadeId = Guid.NewGuid();
            var dto = new FaturamentoDiarioCreateDto 
            { 
                Data = DateOnly.FromDateTime(DateTime.Now) 
            };
            
            // Configura usuário sem claim de tenantId para simular UnauthorizedAccessException
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, _userId.ToString())
                // tenantId claim não está presente
            }, "TestAuth"));
            
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await _controller.SubmeterFechamento(unidadeId, dto);

            // Assert
            var unauthorizedResult = result.Should().BeOfType<UnauthorizedObjectResult>().Subject;
            unauthorizedResult.StatusCode.Should().Be(401);
        }

        [Fact]
        public async Task RevisarFechamento_NaoEncontrado_DeveRetornarNotFound()
        {
            // Arrange
            var unidadeId = Guid.NewGuid();
            var faturamentoId = Guid.NewGuid();
            var updateDto = new FaturamentoDiarioSupervisorUpdateDto();
            
            _serviceMock
                .Setup(s => s.RevisarFechamentoAsync(faturamentoId, updateDto, _userId, _tenantId))
                .ReturnsAsync((null, ErrorMessages.FechamentoNotFound));

            // Act
            var result = await _controller.RevisarFechamento(unidadeId, faturamentoId, updateDto);

            // Assert
            var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
            notFoundResult.StatusCode.Should().Be(404);
            notFoundResult.Value.Should().Be(ErrorMessages.FechamentoNotFound);
        }

        [Fact]
        public async Task RevisarFechamento_Sucesso_DeveRetornarOk()
        {
            // Arrange
            var unidadeId = Guid.NewGuid();
            var faturamentoId = Guid.NewGuid();
            var updateDto = new FaturamentoDiarioSupervisorUpdateDto();
            var responseDto = new FaturamentoDiarioResponseDto 
            { 
                Id = faturamentoId,
                Status = "Aprovado",
                UnidadeId = unidadeId
            };
            
            _serviceMock
                .Setup(s => s.RevisarFechamentoAsync(faturamentoId, updateDto, _userId, _tenantId))
                .ReturnsAsync((responseDto, null));

            // Act
            var result = await _controller.RevisarFechamento(unidadeId, faturamentoId, updateDto);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().Be(responseDto);
        }

        [Fact]
        public async Task RevisarFechamento_IdVazio_DeveRetornarBadRequest()
        {
            // Arrange
            var unidadeId = Guid.NewGuid();
            var updateDto = new FaturamentoDiarioSupervisorUpdateDto();

            // Act
            var result = await _controller.RevisarFechamento(unidadeId, Guid.Empty, updateDto);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("ID do fechamento é inválido.");
        }

        [Fact]
        public async Task GetFechamentoById_Encontrado_DeveRetornarOk()
        {
            // Arrange
            var unidadeId = Guid.NewGuid();
            var faturamentoId = Guid.NewGuid();
            var responseDto = new FaturamentoDiarioResponseDto 
            { 
                Id = faturamentoId,
                Data = DateOnly.FromDateTime(DateTime.Now.AddDays(-2)),
                Status = "Pendente",
                UnidadeId = unidadeId
            };
            
            _serviceMock
                .Setup(s => s.GetFechamentoByIdAsync(faturamentoId, _tenantId))
                .ReturnsAsync(responseDto);

            // Act
            var result = await _controller.GetFechamentoById(unidadeId, faturamentoId);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().Be(responseDto);
        }

        [Fact]
        public async Task GetFechamentoById_NaoEncontrado_DeveRetornarNotFound()
        {
            // Arrange
            var unidadeId = Guid.NewGuid();
            var faturamentoId = Guid.NewGuid();
            
            _serviceMock
                .Setup(s => s.GetFechamentoByIdAsync(faturamentoId, _tenantId))
                .ReturnsAsync((FaturamentoDiarioResponseDto?)null);

            // Act
            var result = await _controller.GetFechamentoById(unidadeId, faturamentoId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetFechamentoById_PertenceOutraUnidade_DeveRetornarNotFound()
        {
            // Arrange
            var unidadeId = Guid.NewGuid();
            var outraUnidadeId = Guid.NewGuid();
            var faturamentoId = Guid.NewGuid();
            var responseDto = new FaturamentoDiarioResponseDto 
            { 
                Id = faturamentoId,
                UnidadeId = outraUnidadeId // Pertence a outra unidade
            };
            
            _serviceMock
                .Setup(s => s.GetFechamentoByIdAsync(faturamentoId, _tenantId))
                .ReturnsAsync(responseDto);

            // Act
            var result = await _controller.GetFechamentoById(unidadeId, faturamentoId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetFechamentosPendentes_Sucesso_DeveRetornarOk()
        {
            // Arrange
            var fechamentos = new List<FaturamentoDiarioResponseDto>
            {
                new() { Id = Guid.NewGuid(), Status = "Pendente" },
                new() { Id = Guid.NewGuid(), Status = "Pendente" }
            };
            
            _serviceMock
                .Setup(s => s.GetFechamentosPendentesAsync(_tenantId))
                .ReturnsAsync(fechamentos);

            // Act
            var result = await _controller.GetFechamentosPendentes();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(fechamentos);
        }

        [Fact]
        public async Task GetFechamentosDaUnidade_Sucesso_DeveRetornarOk()
        {
            // Arrange
            var unidadeId = Guid.NewGuid();
            var fechamentos = new List<FaturamentoDiarioResponseDto>
            {
                new() { Id = Guid.NewGuid(), UnidadeId = unidadeId },
                new() { Id = Guid.NewGuid(), UnidadeId = unidadeId }
            };
            
            _serviceMock
                .Setup(s => s.GetFechamentosPorUnidadeAsync(unidadeId, _tenantId))
                .ReturnsAsync(fechamentos);

            // Act
            var result = await _controller.GetFechamentosDaUnidade(unidadeId);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(fechamentos);
        }

        [Fact]
        public void HandleErrorResponse_MensagemVazia_DeveRetornarBadRequest()
        {
            // Arrange
            var controller = new FaturamentoDiarioController(_serviceMock.Object, _loggerMock.Object);
            var metodo = controller.GetType().GetMethod("HandleErrorResponse", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Act
            var result = metodo?.Invoke(controller, new object[] { "" }) as IActionResult;

            // Assert
            result.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public void HandleErrorResponse_ContemJaExiste_DeveRetornarConflict()
        {
            // Arrange
            var controller = new FaturamentoDiarioController(_serviceMock.Object, _loggerMock.Object);
            var metodo = controller.GetType().GetMethod("HandleErrorResponse", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Act
            var result = metodo?.Invoke(controller, new object[] { "Fechamento já existe para esta data" }) as IActionResult;

            // Assert
            var conflictResult = result.Should().BeOfType<ConflictObjectResult>().Subject;
            conflictResult.Value.Should().Be("Fechamento já existe para esta data");
        }

        [Fact]
        public void HandleErrorResponse_ContemNaoEncontrado_DeveRetornarNotFound()
        {
            // Arrange
            var controller = new FaturamentoDiarioController(_serviceMock.Object, _loggerMock.Object);
            var metodo = controller.GetType().GetMethod("HandleErrorResponse", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Act
            var result = metodo?.Invoke(controller, new object[] { "Fechamento não encontrado" }) as IActionResult;

            // Assert
            var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
            notFoundResult.Value.Should().Be("Fechamento não encontrado");
        }

        private void ConfigurarContextoUsuario(Guid userId, Guid tenantId)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim("tenantId", tenantId.ToString())
            }, "TestAuth"));
            
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }
    }
}