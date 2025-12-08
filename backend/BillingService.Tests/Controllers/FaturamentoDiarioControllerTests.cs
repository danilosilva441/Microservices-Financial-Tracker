// using BillingService.Controllers;
// using BillingService.DTOs;
// using BillingService.Services.Interfaces;
// using FluentAssertions;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Mvc;
// using Moq;
// using SharedKernel;
// using System.Security.Claims;
// using Xunit;

// namespace BillingService.Tests.Controllers
// {
//     public class FaturamentoDiarioControllerTests
//     {
//         private readonly Mock<IFaturamentoDiarioService> _serviceMock;
//         private readonly FaturamentoDiarioController _controller;

//         public FaturamentoDiarioControllerTests()
//         {
//             _serviceMock = new Mock<IFaturamentoDiarioService>();
//             _controller = new FaturamentoDiarioController(_serviceMock.Object);
            
//             // Configura um utilizador padrão para os testes
//             var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
//             {
//                 new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
//                 new Claim("tenantId", Guid.NewGuid().ToString())
//             }, "TestAuth"));
            
//             _controller.ControllerContext = new ControllerContext
//             {
//                 HttpContext = new DefaultHttpContext { User = user }
//             };
//         }

//         [Fact]
//         public async Task SubmeterFechamento_Sucesso_DeveRetornarCreated()
//         {
//             // Arrange
//             var dto = new FaturamentoDiarioCreateDto { Data = DateOnly.FromDateTime(DateTime.Now) };
//             var responseDto = new FaturamentoDiarioResponseDto { Id = Guid.NewGuid() };

//             _serviceMock.Setup(s => s.SubmeterFechamentoAsync(It.IsAny<Guid>(), dto, It.IsAny<Guid>(), It.IsAny<Guid>()))
//                 .ReturnsAsync((responseDto, null)); // Sucesso, sem erro

//             // Act
//             var result = await _controller.SubmeterFechamento(Guid.NewGuid(), dto);

//             // Assert
//             var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
//             createdResult.StatusCode.Should().Be(201);
//         }

//         [Fact]
//         public async Task SubmeterFechamento_Duplicado_DeveRetornarConflict()
//         {
//             // Arrange
//             var dto = new FaturamentoDiarioCreateDto();
//             _serviceMock.Setup(s => s.SubmeterFechamentoAsync(It.IsAny<Guid>(), dto, It.IsAny<Guid>(), It.IsAny<Guid>()))
//                 .ReturnsAsync((null, ErrorMessages.FechamentoJaExiste)); // Erro simulado

//             // Act
//             var result = await _controller.SubmeterFechamento(Guid.NewGuid(), dto);

//             // Assert
//             var conflictResult = result.Should().BeOfType<ConflictObjectResult>().Subject;
//             conflictResult.StatusCode.Should().Be(409);
//             conflictResult.Value.Should().Be(ErrorMessages.FechamentoJaExiste);
//         }

//         [Fact]
//         public async Task RevisarFechamento_NaoEncontrado_DeveRetornarNotFound()
//         {
//             // Arrange
//             var updateDto = new FaturamentoDiarioSupervisorUpdateDto();
//             _serviceMock.Setup(s => s.RevisarFechamentoAsync(It.IsAny<Guid>(), updateDto, It.IsAny<Guid>(), It.IsAny<Guid>()))
//                 .ReturnsAsync((null, ErrorMessages.FechamentoNotFound));

//             // Act
//             var result = await _controller.RevisarFechamento(Guid.NewGuid(), Guid.NewGuid(), updateDto);

//             // Assert
//             result.Should().BeOfType<NotFoundObjectResult>();
//         }
//     }
// }