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
//     public class SolicitacoesControllerTests
//     {
//         private readonly Mock<ISolicitacaoService> _serviceMock;
//         private readonly SolicitacoesController _controller;

//         public SolicitacoesControllerTests()
//         {
//             _serviceMock = new Mock<ISolicitacaoService>();
//             _controller = new SolicitacoesController(_serviceMock.Object);

//             var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
//             {
//                 new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
//             }, "TestAuth"));
//             _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };
//         }

//         [Fact]
//         public async Task CriarSolicitacao_Sucesso_DeveRetornarOk()
//         {
//             var dto = new CriarSolicitacaoDto { Motivo = "Erro" };
//             _serviceMock.Setup(s => s.CriarSolicitacaoAsync(It.IsAny<SolicitacaoAjuste>(), It.IsAny<Guid>()))
//                 .ReturnsAsync(new SolicitacaoAjuste());

//             var result = await _controller.CriarSolicitacao(dto);

//             result.Should().BeOfType<OkObjectResult>();
//         }

//         [Fact]
//         public async Task RevisarSolicitacao_Falha_DeveRetornarBadRequest()
//         {
//             var dto = new SolicitacoesController.RevisaoDto { Acao = "Rejeitar" };
//             _serviceMock.Setup(s => s.RevisarSolicitacaoAsync(It.IsAny<Guid>(), dto.Acao, It.IsAny<Guid>()))
//                 .ReturnsAsync((false, "Erro ao revisar"));

//             var result = await _controller.RevisarSolicitacao(Guid.NewGuid(), dto);

//             var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
//             badRequest.Value.Should().Be("Erro ao revisar");
//         }
//     }
// }