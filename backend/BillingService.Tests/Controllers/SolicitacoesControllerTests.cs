using BillingService.Controllers;
using BillingService.DTOs;
using BillingService.Models;
using BillingService.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Xunit;

namespace BillingService.Tests.Controllers
{
    public class SolicitacoesControllerTests
    {
        private readonly Mock<ISolicitacaoService> _serviceMock;
        private readonly Mock<ILogger<SolicitacoesController>> _loggerMock;
        private readonly SolicitacoesController _controller;
        private readonly Guid _usuarioId;

        public SolicitacoesControllerTests()
        {
            _serviceMock = new Mock<ISolicitacaoService>();
            _loggerMock = new Mock<ILogger<SolicitacoesController>>();
            _controller = new SolicitacoesController(_serviceMock.Object, _loggerMock.Object);

            _usuarioId = Guid.NewGuid();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, _usuarioId.ToString()),
                new Claim(ClaimTypes.Role, "Admin")
            }, "TestAuth"));
            
            _controller.ControllerContext = new ControllerContext 
            { 
                HttpContext = new DefaultHttpContext { User = user } 
            };
        }

        [Fact]
        public async Task CriarSolicitacao_Sucesso_DeveRetornarCreated()
        {
            // Arrange
            var dto = new CriarSolicitacaoDto 
            { 
                FaturamentoParcialId = Guid.NewGuid(),
                Tipo = "alteracao",
                Motivo = "Erro na cobrança",
                DadosAntigos = "{\"valor\":100}",
                DadosNovos = "{\"valor\":90}"
            };
            
            var solicitacaoEsperada = new SolicitacaoAjuste
            {
                Id = Guid.NewGuid(),
                FaturamentoParcialId = dto.FaturamentoParcialId,
                Tipo = dto.Tipo,
                Motivo = dto.Motivo,
                Status = "PENDENTE"
            };

            _serviceMock
                .Setup(s => s.CriarSolicitacaoAsync(
                    It.Is<SolicitacaoAjuste>(s => 
                        s.FaturamentoParcialId == dto.FaturamentoParcialId &&
                        s.Tipo == dto.Tipo &&
                        s.Motivo == dto.Motivo),
                    It.Is<Guid>(id => id == _usuarioId)))
                .ReturnsAsync(solicitacaoEsperada);

            // Act
            var result = await _controller.CriarSolicitacao(dto);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = result as CreatedAtActionResult;
            createdResult!.ActionName.Should().Be("GetSolicitacaoPorId");
            createdResult.Value.Should().BeEquivalentTo(solicitacaoEsperada);
            
            _serviceMock.Verify(
                s => s.CriarSolicitacaoAsync(
                    It.IsAny<SolicitacaoAjuste>(), 
                    It.IsAny<Guid>()),
                Times.Once);
        }

        [Fact]
        public async Task CriarSolicitacao_UsuarioNaoAutenticado_DeveRetornarUnauthorized()
        {
            // Arrange
            var dto = new CriarSolicitacaoDto 
            { 
                FaturamentoParcialId = Guid.NewGuid(),
                Tipo = "alteracao",
                Motivo = "Erro"
            };

            // Remove o usuário do contexto
            _controller.ControllerContext = new ControllerContext 
            { 
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() } 
            };

            // Act
            var result = await _controller.CriarSolicitacao(dto);

            // Assert - Verifica se retorna UnauthorizedObjectResult (com mensagem)
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task CriarSolicitacao_DadosInvalidos_DeveRetornarBadRequest()
        {
            // Arrange
            var dto = new CriarSolicitacaoDto 
            { 
                FaturamentoParcialId = Guid.NewGuid(),
                Tipo = "alteracao",
                Motivo = "Erro"
            };

            _serviceMock
                .Setup(s => s.CriarSolicitacaoAsync(It.IsAny<SolicitacaoAjuste>(), It.IsAny<Guid>()))
                .ThrowsAsync(new ValidationException("Dados inválidos"));

            // Act
            var result = await _controller.CriarSolicitacao(dto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequest = result as BadRequestObjectResult;
            badRequest!.Value.Should().NotBeNull();
        }

        [Fact]
        public async Task CriarSolicitacao_ErroInterno_DeveRetornarInternalServerError()
        {
            // Arrange
            var dto = new CriarSolicitacaoDto 
            { 
                FaturamentoParcialId = Guid.NewGuid(),
                Tipo = "alteracao",
                Motivo = "Erro"
            };

            _serviceMock
                .Setup(s => s.CriarSolicitacaoAsync(It.IsAny<SolicitacaoAjuste>(), It.IsAny<Guid>()))
                .ThrowsAsync(new Exception("Erro interno"));

            // Act
            var result = await _controller.CriarSolicitacao(dto);

            // Assert
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [Fact]
        public async Task RevisarSolicitacao_Sucesso_DeveRetornarNoContent()
        {
            // Arrange
            var solicitacaoId = Guid.NewGuid();
            var dto = new RevisarSolicitacaoDto 
            { 
                Acao = "APROVAR",
                Justificativa = "Solicitação aprovada após análise"
            };

            _serviceMock
                .Setup(s => s.RevisarSolicitacaoAsync(
                    It.Is<Guid>(id => id == solicitacaoId),
                    It.Is<string>(acao => acao == dto.Acao),
                    It.Is<Guid>(id => id == _usuarioId)))
                .ReturnsAsync((true, "Solicitação revisada com sucesso"));

            // Act
            var result = await _controller.RevisarSolicitacao(solicitacaoId, dto);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            
            _serviceMock.Verify(
                s => s.RevisarSolicitacaoAsync(
                    It.IsAny<Guid>(), 
                    It.IsAny<string>(), 
                    It.IsAny<Guid>()),
                Times.Once);
        }

        [Fact]
        public async Task RevisarSolicitacao_Falha_DeveRetornarBadRequest()
        {
            // Arrange
            var solicitacaoId = Guid.NewGuid();
            var dto = new RevisarSolicitacaoDto 
            { 
                Acao = "REJEITAR",
                Justificativa = "Dados inconsistentes"
            };

            _serviceMock
                .Setup(s => s.RevisarSolicitacaoAsync(
                    It.IsAny<Guid>(), 
                    dto.Acao, 
                    It.IsAny<Guid>()))
                .ReturnsAsync((false, "Erro ao revisar solicitação"));

            // Act
            var result = await _controller.RevisarSolicitacao(solicitacaoId, dto);

            // Assert
            var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequest.Value.Should().NotBeNull();
        }

        [Fact]
        public async Task RevisarSolicitacao_AcaoInvalida_DeveRetornarBadRequest()
        {
            // Arrange
            var solicitacaoId = Guid.NewGuid();
            var dto = new RevisarSolicitacaoDto 
            { 
                Acao = "ACAO_INVALIDA", // Não está no regex ^(APROVAR|REJEITAR)$
                Justificativa = "Teste"
            };

            // Act
            var result = await _controller.RevisarSolicitacao(solicitacaoId, dto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task RevisarSolicitacao_ErroInterno_DeveRetornarInternalServerError()
        {
            // Arrange
            var solicitacaoId = Guid.NewGuid();
            var dto = new RevisarSolicitacaoDto 
            { 
                Acao = "APROVAR"
            };

            _serviceMock
                .Setup(s => s.RevisarSolicitacaoAsync(
                    It.IsAny<Guid>(), 
                    It.IsAny<string>(), 
                    It.IsAny<Guid>()))
                .ThrowsAsync(new Exception("Erro interno"));

            // Act
            var result = await _controller.RevisarSolicitacao(solicitacaoId, dto);

            // Assert
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [Fact]
        public async Task GetSolicitacoes_Sucesso_DeveRetornarOk()
        {
            // Arrange - Criando uma lista de SolicitacaoAjusteDto
            var solicitacoes = new List<SolicitacaoAjusteDto>
            {
                new SolicitacaoAjusteDto 
                { 
                    Id = Guid.NewGuid(),
                    Status = "PENDENTE",
                    Tipo = "alteracao",
                    Motivo = "Teste 1",
                    DataSolicitacao = DateTime.UtcNow,
                    Faturamento = new FaturamentoSimplesDto
                    {
                        Id = Guid.NewGuid(),
                        Data = DateTime.UtcNow,
                        Valor = 100.50m,
                        Operacao = new OperacaoSimplesDto
                        {
                            Id = Guid.NewGuid(),
                            Nome = "Operação Teste"
                        }
                    }
                },
                new SolicitacaoAjusteDto 
                { 
                    Id = Guid.NewGuid(),
                    Status = "APROVADO",
                    Tipo = "remocao",
                    Motivo = "Teste 2",
                    DataSolicitacao = DateTime.UtcNow,
                    Faturamento = new FaturamentoSimplesDto
                    {
                        Id = Guid.NewGuid(),
                        Data = DateTime.UtcNow,
                        Valor = 200.75m,
                        Operacao = new OperacaoSimplesDto
                        {
                            Id = Guid.NewGuid(),
                            Nome = "Operação Teste 2"
                        }
                    }
                }
            };

            _serviceMock
                .Setup(s => s.GetSolicitacoesAsync())
                .ReturnsAsync(solicitacoes);

            // Act
            var result = await _controller.GetSolicitacoes();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(solicitacoes);
        }

        [Fact]
        public async Task GetSolicitacoes_ComFiltros_DeveRetornarFiltrado()
        {
            // Arrange
            var solicitacoes = new List<SolicitacaoAjusteDto>
            {
                new SolicitacaoAjusteDto 
                { 
                    Id = Guid.NewGuid(), 
                    Status = "PENDENTE", 
                    Tipo = "alteracao",
                    DataSolicitacao = DateTime.UtcNow,
                    Faturamento = new FaturamentoSimplesDto()
                },
                new SolicitacaoAjusteDto 
                { 
                    Id = Guid.NewGuid(), 
                    Status = "APROVADO", 
                    Tipo = "alteracao",
                    DataSolicitacao = DateTime.UtcNow,
                    Faturamento = new FaturamentoSimplesDto()
                },
                new SolicitacaoAjusteDto 
                { 
                    Id = Guid.NewGuid(), 
                    Status = "PENDENTE", 
                    Tipo = "remocao",
                    DataSolicitacao = DateTime.UtcNow,
                    Faturamento = new FaturamentoSimplesDto()
                }
            };

            _serviceMock
                .Setup(s => s.GetSolicitacoesAsync())
                .ReturnsAsync(solicitacoes);

            // Act - Filtra por status
            var result = await _controller.GetSolicitacoes(status: "PENDENTE");

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var resultadoFiltrado = okResult!.Value as IEnumerable<SolicitacaoAjusteDto>;
            resultadoFiltrado.Should().HaveCount(2);
            resultadoFiltrado.Should().OnlyContain(s => s.Status == "PENDENTE");
        }

        [Fact]
        public async Task GetSolicitacoes_ErroInterno_DeveRetornarInternalServerError()
        {
            // Arrange
            _serviceMock
                .Setup(s => s.GetSolicitacoesAsync())
                .ThrowsAsync(new Exception("Erro interno"));

            // Act
            var result = await _controller.GetSolicitacoes();

            // Assert
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [Fact]
        public async Task GetSolicitacaoPorId_Sucesso_DeveRetornarOk()
        {
            // Arrange
            var solicitacaoId = Guid.NewGuid();
            var solicitacaoEsperada = new SolicitacaoAjusteDto
            {
                Id = solicitacaoId,
                Status = "PENDENTE",
                Tipo = "alteracao",
                Motivo = "Teste",
                DataSolicitacao = DateTime.UtcNow,
                Faturamento = new FaturamentoSimplesDto()
            };

            var solicitacoes = new List<SolicitacaoAjusteDto> { solicitacaoEsperada };

            _serviceMock
                .Setup(s => s.GetSolicitacoesAsync())
                .ReturnsAsync(solicitacoes);

            // Act
            var result = await _controller.GetSolicitacaoPorId(solicitacaoId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(solicitacaoEsperada);
        }

        [Fact]
        public async Task GetSolicitacaoPorId_NaoEncontrada_DeveRetornarNotFound()
        {
            // Arrange
            var solicitacaoId = Guid.NewGuid();

            _serviceMock
                .Setup(s => s.GetSolicitacoesAsync())
                .ReturnsAsync(new List<SolicitacaoAjusteDto>());

            // Act
            var result = await _controller.GetSolicitacaoPorId(solicitacaoId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetSolicitacaoPorId_ErroInterno_DeveRetornarInternalServerError()
        {
            // Arrange
            var solicitacaoId = Guid.NewGuid();

            _serviceMock
                .Setup(s => s.GetSolicitacoesAsync())
                .ThrowsAsync(new Exception("Erro interno"));

            // Act
            var result = await _controller.GetSolicitacaoPorId(solicitacaoId);

            // Assert
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [Fact]
        public async Task GetMinhasSolicitacoes_Sucesso_DeveRetornarOk()
        {
            // Arrange
            var solicitacoes = new List<SolicitacaoAjusteDto>
            {
                new SolicitacaoAjusteDto 
                { 
                    Id = Guid.NewGuid(), 
                    Status = "PENDENTE",
                    DataSolicitacao = DateTime.UtcNow,
                    Faturamento = new FaturamentoSimplesDto()
                },
                new SolicitacaoAjusteDto 
                { 
                    Id = Guid.NewGuid(), 
                    Status = "APROVADO",
                    DataSolicitacao = DateTime.UtcNow,
                    Faturamento = new FaturamentoSimplesDto()
                }
            };

            _serviceMock
                .Setup(s => s.GetSolicitacoesAsync())
                .ReturnsAsync(solicitacoes);

            // Act
            var result = await _controller.GetMinhasSolicitacoes();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(solicitacoes);
        }

        [Fact]
        public async Task GetMinhasSolicitacoes_ComFiltroStatus_DeveRetornarFiltrado()
        {
            // Arrange
            var solicitacoes = new List<SolicitacaoAjusteDto>
            {
                new SolicitacaoAjusteDto 
                { 
                    Id = Guid.NewGuid(), 
                    Status = "PENDENTE",
                    DataSolicitacao = DateTime.UtcNow,
                    Faturamento = new FaturamentoSimplesDto()
                },
                new SolicitacaoAjusteDto 
                { 
                    Id = Guid.NewGuid(), 
                    Status = "APROVADO",
                    DataSolicitacao = DateTime.UtcNow,
                    Faturamento = new FaturamentoSimplesDto()
                },
                new SolicitacaoAjusteDto 
                { 
                    Id = Guid.NewGuid(), 
                    Status = "PENDENTE",
                    DataSolicitacao = DateTime.UtcNow,
                    Faturamento = new FaturamentoSimplesDto()
                }
            };

            _serviceMock
                .Setup(s => s.GetSolicitacoesAsync())
                .ReturnsAsync(solicitacoes);

            // Act
            var result = await _controller.GetMinhasSolicitacoes(status: "PENDENTE");

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var resultadoFiltrado = okResult!.Value as IEnumerable<SolicitacaoAjusteDto>;
            resultadoFiltrado.Should().HaveCount(2);
            resultadoFiltrado.Should().OnlyContain(s => s.Status == "PENDENTE");
        }

        [Fact]
        public async Task GetMinhasSolicitacoes_UsuarioNaoAutenticado_DeveRetornarUnauthorized()
        {
            // Arrange
            // Remove o usuário do contexto
            _controller.ControllerContext = new ControllerContext 
            { 
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() } 
            };

            // Act
            var result = await _controller.GetMinhasSolicitacoes();

            // Assert - Verifica se retorna UnauthorizedObjectResult (com mensagem)
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task GetMinhasSolicitacoes_ErroInterno_DeveRetornarInternalServerError()
        {
            // Arrange
            _serviceMock
                .Setup(s => s.GetSolicitacoesAsync())
                .ThrowsAsync(new Exception("Erro interno"));

            // Act
            var result = await _controller.GetMinhasSolicitacoes();

            // Assert
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }
    }
}