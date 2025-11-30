using BillingService.Models;
using BillingService.Repositories.Interfaces;
using BillingService.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace BillingService.Tests.Services
{
    public class SolicitacaoServiceTests
    {
        private readonly Mock<ISolicitacaoRepository> _repoMock;
        private readonly Mock<IFaturamentoParcialRepository> _fatRepoMock;
        private readonly Mock<IUnidadeRepository> _unidadeRepoMock;
        private readonly SolicitacaoService _service;

        public SolicitacaoServiceTests()
        {
            _repoMock = new Mock<ISolicitacaoRepository>();
            _fatRepoMock = new Mock<IFaturamentoParcialRepository>();
            _unidadeRepoMock = new Mock<IUnidadeRepository>();
            _service = new SolicitacaoService(_repoMock.Object, _fatRepoMock.Object, _unidadeRepoMock.Object);
        }

        [Fact]
        public async Task RevisarSolicitacaoAsync_AprovarRemocao_DeveDesativarFaturamento()
        {
            // Arrange
            var id = Guid.NewGuid();
            var solicitacao = new SolicitacaoAjuste
            {
                Id = id,
                Status = "PENDENTE",
                Tipo = "remocao",
                FaturamentoParcial = new FaturamentoParcial { IsAtivo = true }
            };

            _repoMock.Setup(r => r.GetByIdComFaturamentoAsync(id)).ReturnsAsync(solicitacao);

            // Act
            var (success, _) = await _service.RevisarSolicitacaoAsync(id, "APROVADA", Guid.NewGuid());

            // Assert
            success.Should().BeTrue();
            solicitacao.Status.Should().Be("APROVADA");
            solicitacao.FaturamentoParcial.IsAtivo.Should().BeFalse("pois a solicitação de remoção foi aprovada");
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}