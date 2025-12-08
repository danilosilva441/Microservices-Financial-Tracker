// using BillingService.Controllers;
// using BillingService.DTOs;
// using BillingService.Models;
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
//     public class FaturamentoParcialControllerTests
//     {
//         private readonly Mock<IFaturamentoParcialService> _serviceMock;
//         private readonly FaturamentoParcialController _controller;

//         public FaturamentoParcialControllerTests()
//         {
//             _serviceMock = new Mock<IFaturamentoParcialService>();
//             _controller = new FaturamentoParcialController(_serviceMock.Object);

//             // Simula Utilizador Logado
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
//         public async Task AddFaturamentoParcial_Sucesso_DeveRetornarOk()
//         {
//             // Arrange
//             var dto = new FaturamentoParcialCreateDto { Valor = 50 };
//             var novoFaturamento = new FaturamentoParcial { Id = Guid.NewGuid(), Valor = 50 };

//             _serviceMock.Setup(s => s.AddFaturamentoAsync(It.IsAny<Guid>(), dto, It.IsAny<Guid>(), It.IsAny<Guid>()))
//                 .ReturnsAsync((novoFaturamento, null));

//             // Act
//             var result = await _controller.AddFaturamentoParcial(Guid.NewGuid(), dto);

//             // Assert
//             var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
//             okResult.Value.Should().Be(novoFaturamento);
//         }

//         [Fact]
//         public async Task AddFaturamentoParcial_Duplicado_DeveRetornarConflict()
//         {
//             // Arrange
//             _serviceMock.Setup(s => s.AddFaturamentoAsync(It.IsAny<Guid>(), It.IsAny<FaturamentoParcialCreateDto>(), It.IsAny<Guid>(), It.IsAny<Guid>()))
//                 .ReturnsAsync((null, ErrorMessages.FechamentoJaExiste));

//             // Act
//             var result = await _controller.AddFaturamentoParcial(Guid.NewGuid(), new FaturamentoParcialCreateDto());

//             // Assert
//             result.Should().BeOfType<ConflictObjectResult>();
//         }
//     }
// }