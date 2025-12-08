using BillingService.Models;
using BillingService.Repositories.Interfaces;
using BillingService.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BillingService.Tests.Services
{
    public class SolicitacaoServiceTests
    {
        private readonly Mock<ISolicitacaoRepository> _solicitacaoRepositoryMock;
        private readonly Mock<IFaturamentoParcialRepository> _faturamentoRepositoryMock;
        private readonly Mock<IUnidadeRepository> _unidadeRepositoryMock;
        private readonly Mock<ILogger<SolicitacaoService>> _loggerMock;
        private readonly SolicitacaoService _service;
        private readonly Guid _usuarioRevisorId = Guid.NewGuid();

        public SolicitacaoServiceTests()
        {
            _solicitacaoRepositoryMock = new Mock<ISolicitacaoRepository>();
            _faturamentoRepositoryMock = new Mock<IFaturamentoParcialRepository>();
            _unidadeRepositoryMock = new Mock<IUnidadeRepository>();
            _loggerMock = new Mock<ILogger<SolicitacaoService>>();
            _service = new SolicitacaoService(
                _solicitacaoRepositoryMock.Object, 
                _faturamentoRepositoryMock.Object, 
                _unidadeRepositoryMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task RevisarSolicitacaoAsync_AprovarRemocao_DeveDesativarFaturamento()
        {
            // Arrange
            var solicitacaoId = Guid.NewGuid();
            var faturamentoParcial = new FaturamentoParcial 
            { 
                Id = Guid.NewGuid(),
                IsAtivo = true 
            };
            
            var solicitacao = new SolicitacaoAjuste
            {
                Id = solicitacaoId,
                Status = "PENDENTE",
                Tipo = "remocao",
                FaturamentoParcial = faturamentoParcial,
                FaturamentoParcialId = faturamentoParcial.Id
            };

            _solicitacaoRepositoryMock
                .Setup(r => r.GetByIdComFaturamentoAsync(solicitacaoId))
                .ReturnsAsync(solicitacao);

            _solicitacaoRepositoryMock
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var (success, mensagem) = await _service.RevisarSolicitacaoAsync(
                solicitacaoId, 
                "APROVADA", 
                _usuarioRevisorId
            );

            // Assert
            success.Should().BeTrue();
            mensagem.Should().BeNull();
            
            solicitacao.Status.Should().Be("APROVADA");
            solicitacao.FaturamentoParcial.IsAtivo.Should().BeFalse();
            
            _solicitacaoRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task RevisarSolicitacaoAsync_SolicitacaoNaoEncontrada_DeveRetornarFalso()
        {
            // Arrange
            var solicitacaoId = Guid.NewGuid();
            
            _solicitacaoRepositoryMock
                .Setup(r => r.GetByIdComFaturamentoAsync(solicitacaoId))
                .ReturnsAsync((SolicitacaoAjuste?)null);

            // Act
            var (success, mensagem) = await _service.RevisarSolicitacaoAsync(
                solicitacaoId, 
                "APROVADA", 
                _usuarioRevisorId
            );

            // Assert
            success.Should().BeFalse();
            mensagem.Should().NotBeNullOrEmpty();
            _solicitacaoRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task RevisarSolicitacaoAsync_SolicitacaoJaRevisada_DeveRetornarFalso()
        {
            // Arrange
            var solicitacaoId = Guid.NewGuid();
            var solicitacao = new SolicitacaoAjuste
            {
                Id = solicitacaoId,
                Status = "APROVADA", // Já revisada
                Tipo = "remocao",
                FaturamentoParcial = new FaturamentoParcial { IsAtivo = true }
            };

            _solicitacaoRepositoryMock
                .Setup(r => r.GetByIdComFaturamentoAsync(solicitacaoId))
                .ReturnsAsync(solicitacao);

            // Act
            var (success, mensagem) = await _service.RevisarSolicitacaoAsync(
                solicitacaoId, 
                "APROVADA", 
                _usuarioRevisorId
            );

            // Assert
            success.Should().BeFalse();
            mensagem.Should().NotBeNullOrEmpty();
            solicitacao.FaturamentoParcial.IsAtivo.Should().BeTrue();
            _solicitacaoRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Theory]
        [InlineData("REJEITADA")]
        [InlineData("CANCELADA")]
        public async Task RevisarSolicitacaoAsync_StatusNaoAprovado_NaoDeveDesativarFaturamento(string status)
        {
            // Arrange
            var solicitacaoId = Guid.NewGuid();
            var solicitacao = new SolicitacaoAjuste
            {
                Id = solicitacaoId,
                Status = "PENDENTE",
                Tipo = "remocao",
                FaturamentoParcial = new FaturamentoParcial { IsAtivo = true }
            };

            _solicitacaoRepositoryMock
                .Setup(r => r.GetByIdComFaturamentoAsync(solicitacaoId))
                .ReturnsAsync(solicitacao);

            _solicitacaoRepositoryMock
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var (success, mensagem) = await _service.RevisarSolicitacaoAsync(
                solicitacaoId, 
                status, 
                _usuarioRevisorId
            );

            // Assert
            success.Should().BeTrue();
            mensagem.Should().BeNull();
            solicitacao.Status.Should().Be(status);
            solicitacao.FaturamentoParcial.IsAtivo.Should().BeTrue();
            _solicitacaoRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task RevisarSolicitacaoAsync_TipoNaoRemocao_NaoDeveDesativarFaturamento()
        {
            // Arrange
            var solicitacaoId = Guid.NewGuid();
            var solicitacao = new SolicitacaoAjuste
            {
                Id = solicitacaoId,
                Status = "PENDENTE",
                Tipo = "adicao",
                FaturamentoParcial = new FaturamentoParcial { IsAtivo = true }
            };

            _solicitacaoRepositoryMock
                .Setup(r => r.GetByIdComFaturamentoAsync(solicitacaoId))
                .ReturnsAsync(solicitacao);

            _solicitacaoRepositoryMock
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var (success, mensagem) = await _service.RevisarSolicitacaoAsync(
                solicitacaoId, 
                "APROVADA", 
                _usuarioRevisorId
            );

            // Assert
            success.Should().BeTrue();
            mensagem.Should().BeNull();
            solicitacao.Status.Should().Be("APROVADA");
            solicitacao.FaturamentoParcial.IsAtivo.Should().BeTrue();
            _solicitacaoRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task RevisarSolicitacaoAsync_AcaoInvalida_DeveRetornarFalso()
        {
            // Arrange
            var solicitacaoId = Guid.NewGuid();
            
            // Act
            var (success, mensagem) = await _service.RevisarSolicitacaoAsync(
                solicitacaoId, 
                "", // Ação vazia
                _usuarioRevisorId
            );

            // Assert
            success.Should().BeFalse();
            mensagem.Should().NotBeNullOrEmpty();
            _solicitacaoRepositoryMock.Verify(r => r.GetByIdComFaturamentoAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task RevisarSolicitacaoAsync_AcaoInvalidaTexto_DeveRetornarFalso()
        {
            // Arrange
            var solicitacaoId = Guid.NewGuid();
            
            // Act
            var (success, mensagem) = await _service.RevisarSolicitacaoAsync(
                solicitacaoId, 
                "INVALIDO", // Ação inválida
                _usuarioRevisorId
            );

            // Assert
            success.Should().BeFalse();
            mensagem.Should().NotBeNullOrEmpty();
            _solicitacaoRepositoryMock.Verify(r => r.GetByIdComFaturamentoAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task RevisarSolicitacaoAsync_AprovarIdVazio_DeveRetornarFalso()
        {
            // Arrange
            var solicitacaoId = Guid.NewGuid();
            
            // Act
            var (success, mensagem) = await _service.RevisarSolicitacaoAsync(
                solicitacaoId, 
                "APROVADA", 
                Guid.Empty // AprovadorId vazio
            );

            // Assert
            success.Should().BeFalse();
            mensagem.Should().NotBeNullOrEmpty();
            _solicitacaoRepositoryMock.Verify(r => r.GetByIdComFaturamentoAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task RevisarSolicitacaoAsync_AprovarAlteracaoComDadosNovos_DeveAtualizarValorEData()
        {
            // Arrange
            var solicitacaoId = Guid.NewGuid();
            var faturamentoParcial = new FaturamentoParcial 
            { 
                Id = Guid.NewGuid(),
                Valor = 100.0m,
                HoraInicio = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc),
                IsAtivo = true 
            };
            
            var solicitacao = new SolicitacaoAjuste
            {
                Id = solicitacaoId,
                Status = "PENDENTE",
                Tipo = "alteracao",
                DadosNovos = "{\"Valor\": 200.0, \"Data\": \"2024-01-02T11:00:00\"}",
                FaturamentoParcial = faturamentoParcial,
                FaturamentoParcialId = faturamentoParcial.Id
            };

            _solicitacaoRepositoryMock
                .Setup(r => r.GetByIdComFaturamentoAsync(solicitacaoId))
                .ReturnsAsync(solicitacao);

            _solicitacaoRepositoryMock
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var (success, mensagem) = await _service.RevisarSolicitacaoAsync(
                solicitacaoId, 
                "APROVADA", 
                _usuarioRevisorId
            );

            // Assert
            success.Should().BeTrue();
            mensagem.Should().BeNull();
            solicitacao.Status.Should().Be("APROVADA");
            faturamentoParcial.Valor.Should().Be(200.0m);
            faturamentoParcial.HoraInicio.Should().Be(new DateTime(2024, 1, 2, 11, 0, 0, DateTimeKind.Utc));
            _solicitacaoRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task RevisarSolicitacaoAsync_AprovarAlteracaoSemDadosNovos_NaoDeveAtualizar()
        {
            // Arrange
            var solicitacaoId = Guid.NewGuid();
            var faturamentoParcial = new FaturamentoParcial 
            { 
                Id = Guid.NewGuid(),
                Valor = 100.0m,
                HoraInicio = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc),
                IsAtivo = true 
            };
            
            var solicitacao = new SolicitacaoAjuste
            {
                Id = solicitacaoId,
                Status = "PENDENTE",
                Tipo = "alteracao",
                DadosNovos = null, // Sem dados novos
                FaturamentoParcial = faturamentoParcial,
                FaturamentoParcialId = faturamentoParcial.Id
            };

            _solicitacaoRepositoryMock
                .Setup(r => r.GetByIdComFaturamentoAsync(solicitacaoId))
                .ReturnsAsync(solicitacao);

            _solicitacaoRepositoryMock
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var (success, mensagem) = await _service.RevisarSolicitacaoAsync(
                solicitacaoId, 
                "APROVADA", 
                _usuarioRevisorId
            );

            // Assert
            success.Should().BeTrue();
            mensagem.Should().BeNull();
            solicitacao.Status.Should().Be("APROVADA");
            faturamentoParcial.Valor.Should().Be(100.0m); // Mantém o valor original
            faturamentoParcial.HoraInicio.Should().Be(new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc)); // Mantém a data original
            _solicitacaoRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task RevisarSolicitacaoAsync_AprovarAlteracaoComJsonInvalido_DeveRetornarErro()
        {
            // Arrange
            var solicitacaoId = Guid.NewGuid();
            var faturamentoParcial = new FaturamentoParcial 
            { 
                Id = Guid.NewGuid(),
                Valor = 100.0m,
                HoraInicio = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc),
                IsAtivo = true 
            };
            
            var solicitacao = new SolicitacaoAjuste
            {
                Id = solicitacaoId,
                Status = "PENDENTE",
                Tipo = "alteracao",
                DadosNovos = "{invalido json}", // JSON inválido
                FaturamentoParcial = faturamentoParcial,
                FaturamentoParcialId = faturamentoParcial.Id
            };

            _solicitacaoRepositoryMock
                .Setup(r => r.GetByIdComFaturamentoAsync(solicitacaoId))
                .ReturnsAsync(solicitacao);

            // Act
            var (success, mensagem) = await _service.RevisarSolicitacaoAsync(
                solicitacaoId, 
                "APROVADA", 
                _usuarioRevisorId
            );

            // Assert
            success.Should().BeFalse();
            mensagem.Should().NotBeNullOrEmpty();
            mensagem.Should().Contain("Erro ao processar os dados");
            faturamentoParcial.Valor.Should().Be(100.0m); // Mantém o valor original
            faturamentoParcial.HoraInicio.Should().Be(new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc)); // Mantém a data original
            _solicitacaoRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task CriarSolicitacaoAsync_ComDadosValidos_DeveCriarSolicitacao()
        {
            // Arrange
            var solicitanteId = Guid.NewGuid();
            var solicitacao = new SolicitacaoAjuste
            {
                Tipo = "adicao",
                Motivo = "Teste de criação",
                FaturamentoParcialId = Guid.NewGuid()
            };

            _solicitacaoRepositoryMock
                .Setup(r => r.AddAsync(solicitacao))
                .Returns(Task.CompletedTask);

            _solicitacaoRepositoryMock
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.CriarSolicitacaoAsync(solicitacao, solicitanteId);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be("PENDENTE");
            result.SolicitanteId.Should().Be(solicitanteId);
            result.DataSolicitacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            _solicitacaoRepositoryMock.Verify(r => r.AddAsync(solicitacao), Times.Once);
            _solicitacaoRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CriarSolicitacaoAsync_ComSolicitacaoNula_DeveLancarExcecao()
        {
            // Arrange
            var solicitanteId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _service.CriarSolicitacaoAsync(null!, solicitanteId));
        }

        [Fact]
        public async Task CriarSolicitacaoAsync_ComSolicitanteIdVazio_DeveLancarExcecao()
        {
            // Arrange
            var solicitacao = new SolicitacaoAjuste
            {
                Tipo = "adicao",
                Motivo = "Teste"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.CriarSolicitacaoAsync(solicitacao, Guid.Empty));
        }

        [Fact]
        public async Task GetSolicitacoesAsync_DeveRetornarListaDeSolicitacoes()
        {
            // Arrange
            var solicitacoes = new List<SolicitacaoAjuste>
            {
                new SolicitacaoAjuste
                {
                    Id = Guid.NewGuid(),
                    Status = "PENDENTE",
                    Tipo = "adicao",
                    DataSolicitacao = DateTime.UtcNow
                }
            };

            _solicitacaoRepositoryMock
                .Setup(r => r.GetAllComDetalhesAsync())
                .ReturnsAsync(solicitacoes);

            // Act
            var result = await _service.GetSolicitacoesAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            _solicitacaoRepositoryMock.Verify(r => r.GetAllComDetalhesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetSolicitacaoByIdAsync_ComIdValido_DeveRetornarSolicitacao()
        {
            // Arrange
            var solicitacaoId = Guid.NewGuid();
            var solicitacao = new SolicitacaoAjuste
            {
                Id = solicitacaoId,
                Status = "PENDENTE",
                Tipo = "adicao",
                DataSolicitacao = DateTime.UtcNow
            };

            _solicitacaoRepositoryMock
                .Setup(r => r.GetByIdComFaturamentoAsync(solicitacaoId))
                .ReturnsAsync(solicitacao);

            // Act
            var result = await _service.GetSolicitacaoByIdAsync(solicitacaoId);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(solicitacaoId);
            _solicitacaoRepositoryMock.Verify(r => r.GetByIdComFaturamentoAsync(solicitacaoId), Times.Once);
        }

        [Fact]
        public async Task GetSolicitacaoByIdAsync_ComIdInexistente_DeveRetornarNull()
        {
            // Arrange
            var solicitacaoId = Guid.NewGuid();

            _solicitacaoRepositoryMock
                .Setup(r => r.GetByIdComFaturamentoAsync(solicitacaoId))
                .ReturnsAsync((SolicitacaoAjuste?)null);

            // Act
            var result = await _service.GetSolicitacaoByIdAsync(solicitacaoId);

            // Assert
            result.Should().BeNull();
            _solicitacaoRepositoryMock.Verify(r => r.GetByIdComFaturamentoAsync(solicitacaoId), Times.Once);
        }
    }
}