// using BillingService.Controllers;
// using BillingService.DTOs;
// using BillingService.Models;
// using BillingService.Services.Interfaces;
// using FluentAssertions;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Mvc;
// using Moq;
// using System.Security.Claims;
// using Xunit;

// namespace BillingService.Tests.Controllers
// {
//     public class MensalistasControllerTests
//     {
//         private readonly Mock<IMensalistaService> _serviceMock;
//         private readonly MensalistasController _controller;

//         public MensalistasControllerTests()
//         {
//             _serviceMock = new Mock<IMensalistaService>();
//             _controller = new MensalistasController(_serviceMock.Object);

//             var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
//             {
//                 new Claim("tenantId", Guid.NewGuid().ToString())
//             }, "TestAuth"));
//             _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };
//         }

//         [Fact]
//         public async Task GetMensalistas_Sucesso_DeveRetornarLista()
//         {
//             _serviceMock.Setup(s => s.GetMensalistasAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
//                 .ReturnsAsync(new List<Mensalista>());

//             var result = await _controller.GetMensalistas(Guid.NewGuid());

//             result.Should().BeOfType<OkObjectResult>();
//         }

//         [Fact]
//         public async Task CreateMensalista_Sucesso_DeveRetornarCreated()
//         {
//             var dto = new CreateMensalistaDto { Nome = "Cliente" };
//             _serviceMock.Setup(s => s.CreateMensalistaAsync(It.IsAny<Guid>(), dto, It.IsAny<Guid>()))
//                 .ReturnsAsync((new Mensalista(), null));

//             var result = await _controller.CreateMensalista(Guid.NewGuid(), dto);

//             result.Should().BeOfType<CreatedAtActionResult>();
//         }
//     }
// }