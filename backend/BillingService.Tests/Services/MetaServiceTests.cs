using BillingService.DTOs;
using BillingService.Models;
using BillingService.Repositories.Interfaces;
using BillingService.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace BillingService.Tests.Services
{
    public class MetaServiceTests
    {
        private readonly Mock<IMetaRepository> _repositoryMock;
        private readonly Mock<IUnidadeRepository> _unidadeRepoMock;
        private readonly MetaService _service;

        public MetaServiceTests()
        {
            _repositoryMock = new Mock<IMetaRepository>();
            _unidadeRepoMock = new Mock<IUnidadeRepository>();
            _service = new MetaService(_repositoryMock.Object, _unidadeRepoMock.Object);
        }

        [Fact]
        public async Task SetMetaAsync_SeMetaNaoExiste_DeveCriarNova()
        {
            // Arrange
            var unidadeId = Guid.NewGuid();
            var tenantId = Guid.NewGuid();
            var dto = new MetaDto { Mes = 11, Ano = 2025, ValorAlvo = 60000 };

            // Mock: Unidade existe
            _unidadeRepoMock.Setup(r => r.GetByIdAsync(unidadeId, tenantId))
                .ReturnsAsync(new Unidade());
            
            // Mock: Meta NÃO existe
            _repositoryMock.Setup(r => r.GetByUnidadeAndPeriodAsync(unidadeId, dto.Mes, dto.Ano, tenantId))
                .ReturnsAsync((Meta?)null);

            // Act
            var (result, error) = await _service.SetMetaAsync(unidadeId, dto, tenantId);

            // Assert
            error.Should().BeNull();
            result.Should().NotBeNull();
            result!.ValorAlvo.Should().Be(60000);
            
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Meta>()), Times.Once);
            _repositoryMock.Verify(r => r.Update(It.IsAny<Meta>()), Times.Never);
        }

        [Fact]
        public async Task SetMetaAsync_SeMetaExiste_DeveAtualizar()
        {
            // Arrange
            var unidadeId = Guid.NewGuid();
            var tenantId = Guid.NewGuid();
            var dto = new MetaDto { Mes = 11, Ano = 2025, ValorAlvo = 70000 };

            var metaExistente = new Meta { Id = Guid.NewGuid(), ValorAlvo = 50000 };

            _unidadeRepoMock.Setup(r => r.GetByIdAsync(unidadeId, tenantId)).ReturnsAsync(new Unidade());
            
            // Mock: Meta EXISTE
            _repositoryMock.Setup(r => r.GetByUnidadeAndPeriodAsync(unidadeId, dto.Mes, dto.Ano, tenantId))
                .ReturnsAsync(metaExistente);

            // Act
            var (result, error) = await _service.SetMetaAsync(unidadeId, dto, tenantId);

            // Assert
            result!.ValorAlvo.Should().Be(70000); // Valor atualizado
            
            _repositoryMock.Verify(r => r.Update(metaExistente), Times.Once);
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Meta>()), Times.Never);
        }
    }
}