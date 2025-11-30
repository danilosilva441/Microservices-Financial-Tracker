using BillingService.DTOs;
using BillingService.Models;
using BillingService.Repositories.Interfaces;
using BillingService.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace BillingService.Tests.Services
{
    public class UnidadeServiceTests
    {
        private readonly Mock<IUnidadeRepository> _repositoryMock;
        private readonly UnidadeService _service;

        public UnidadeServiceTests()
        {
            _repositoryMock = new Mock<IUnidadeRepository>();
            _service = new UnidadeService(_repositoryMock.Object);
        }

        [Fact]
        public async Task CreateUnidadeAsync_DadosValidos_DeveChamarAddAsync()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var dto = new UnidadeDto
            {
                Nome = "Nova Unidade",
                MetaMensal = 50000,
                DataInicio = DateTime.UtcNow
            };

            // Act
            var result = await _service.CreateUnidadeAsync(dto, userId, tenantId);

            // Assert
            result.Should().NotBeNull();
            result.TenantId.Should().Be(tenantId);
            
            // Verifica se salvou a unidade e o vínculo do usuário
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Unidade>()), Times.Once);
            _repositoryMock.Verify(r => r.AddUsuarioOperacaoLinkAsync(It.IsAny<UsuarioOperacao>()), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllUnidadesAsync_DeveChamarMetodoCorretoParaGerente()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            
            // Act
            await _service.GetAllUnidadesByTenantAsync(tenantId);

            // Assert
            // Garante que chamou o método filtrado por Tenant
            _repositoryMock.Verify(r => r.GetAllAsync(tenantId), Times.Once);
            _repositoryMock.Verify(r => r.GetAllAdminAsync(), Times.Never);
        }

        [Fact]
        public async Task GetAllUnidadesAdminAsync_DeveChamarMetodoSemFiltro()
        {
            // Act
            await _service.GetAllUnidadesAdminAsync();

            // Assert
            // Garante que chamou o método de Admin
            _repositoryMock.Verify(r => r.GetAllAdminAsync(), Times.Once);
            _repositoryMock.Verify(r => r.GetAllAsync(It.IsAny<Guid>()), Times.Never);
        }
    }
}