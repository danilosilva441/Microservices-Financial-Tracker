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
//     public class MetasControllerTests
//     {
//         private readonly Mock<IMetaService> _serviceMock;
//         private readonly MetasController _controller;

//         public MetasControllerTests()
//         {
//             _serviceMock = new Mock<IMetaService>();
//             _controller = new MetasController(_serviceMock.Object);

//             var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
//             {
//                 new Claim("tenantId", Guid.NewGuid().ToString())
//             }, "TestAuth"));
//             _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };
//         }

//         [Fact]
//         public async Task GetMetaPorPeriodo_NaoEncontrada_DeveRetornarNotFound()
//         {
//             _serviceMock.Setup(s => s.GetMetaAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Guid>()))
//                 .ReturnsAsync((Meta?)null);

//             var result = await _controller.GetMetaPorPeriodo(Guid.NewGuid(), 1, 2025);

//             result.Should().BeOfType<NotFoundObjectResult>();
//         }

//         [Fact]
//         public async Task SetMeta_Sucesso_DeveRetornarOk()
//         {
//             var dto = new MetaDto { ValorAlvo = 1000 };
//             _serviceMock.Setup(s => s.SetMetaAsync(It.IsAny<Guid>(), dto, It.IsAny<Guid>()))
//                 .ReturnsAsync((new Meta(), null));

//             var result = await _controller.SetMeta(Guid.NewGuid(), dto);

//             result.Should().BeOfType<OkObjectResult>();
//         }
//     }
// }