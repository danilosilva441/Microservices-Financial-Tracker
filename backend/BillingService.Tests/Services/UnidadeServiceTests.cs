using BillingService.DTOs;
using BillingService.Models;
using BillingService.Repositories.Interfaces;
using BillingService.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BillingService.Tests.Services
{
    public class UnidadeServiceTests
    {
        private readonly Mock<IUnidadeRepository> _repositoryMock;
        private readonly Mock<ILogger<UnidadeService>> _loggerMock;
        private readonly UnidadeService _service;

        public UnidadeServiceTests()
        {
            _repositoryMock = new Mock<IUnidadeRepository>();
            _loggerMock = new Mock<ILogger<UnidadeService>>();
            _service = new UnidadeService(_repositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task CreateUnidadeAsync_DadosValidos_DeveCriarUnidadeEVinculoUsuario()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var dto = new UnidadeDto
            {
                Nome = "Nova Unidade",
                Descricao = "Descrição da unidade",
                Endereco = "Endereço da unidade",
                MetaMensal = 50000,
                DataInicio = DateTime.UtcNow
            };

            Unidade unidadeSalva = null!;
            UsuarioOperacao usuarioOperacaoSalvo = null!;

            _repositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Unidade>()))
                .Callback<Unidade>(u => unidadeSalva = u);

            _repositoryMock
                .Setup(r => r.AddUsuarioOperacaoLinkAsync(It.IsAny<UsuarioOperacao>()))
                .Callback<UsuarioOperacao>(uo => usuarioOperacaoSalvo = uo);

            // Act
            var result = await _service.CreateUnidadeAsync(dto, userId, tenantId);

            // Assert
            result.Should().NotBeNull();
            result.TenantId.Should().Be(tenantId);
            result.Nome.Should().Be(dto.Nome);
            result.MetaMensal.Should().Be(dto.MetaMensal);
            result.DataInicio.Should().Be(dto.DataInicio);
            
            // Verifica os parâmetros passados para o repositório
            unidadeSalva.Should().NotBeNull();
            unidadeSalva.TenantId.Should().Be(tenantId);
            unidadeSalva.Nome.Should().Be(dto.Nome);
            
            usuarioOperacaoSalvo.Should().NotBeNull();
            usuarioOperacaoSalvo.UserId.Should().Be(userId);
            usuarioOperacaoSalvo.UnidadeId.Should().Be(result.Id);
            
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Unidade>()), Times.Once);
            _repositoryMock.Verify(r => r.AddUsuarioOperacaoLinkAsync(It.IsAny<UsuarioOperacao>()), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateUnidadeAsync_DadosInvalidos_DeveLancarExcecao()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var dto = new UnidadeDto
            {
                Nome = "", // Nome inválido (vazio)
                MetaMensal = 50000,
                DataInicio = DateTime.UtcNow
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                _service.CreateUnidadeAsync(dto, userId, tenantId));
        }

        [Fact]
        public async Task CreateUnidadeAsync_UserIdVazio_DeveLancarExcecao()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var dto = new UnidadeDto
            {
                Nome = "Nova Unidade",
                MetaMensal = 50000,
                DataInicio = DateTime.UtcNow
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                _service.CreateUnidadeAsync(dto, Guid.Empty, tenantId));
        }

        [Fact]
        public async Task GetAllUnidadesByTenantAsync_TenantIdValido_DeveRetornarUnidadesDoTenant()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var unidadesEsperadas = new List<Unidade>
            {
                new Unidade { Id = Guid.NewGuid(), Nome = "Unidade 1", TenantId = tenantId },
                new Unidade { Id = Guid.NewGuid(), Nome = "Unidade 2", TenantId = tenantId }
            };

            _repositoryMock
                .Setup(r => r.GetAllAsync(tenantId))
                .ReturnsAsync(unidadesEsperadas);

            // Act
            var result = await _service.GetAllUnidadesByTenantAsync(tenantId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(unidadesEsperadas);
            _repositoryMock.Verify(r => r.GetAllAsync(tenantId), Times.Once);
        }

        [Fact]
        public async Task GetAllUnidadesByTenantAsync_TenantIdVazio_DeveLancarExcecao()
        {
            // Arrange
            var tenantId = Guid.Empty;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                _service.GetAllUnidadesByTenantAsync(tenantId));
        }

        [Fact]
        public async Task GetAllUnidadesAdminAsync_DeveRetornarTodasUnidades()
        {
            // Arrange
            var unidadesEsperadas = new List<Unidade>
            {
                new Unidade { Id = Guid.NewGuid(), Nome = "Unidade 1", TenantId = Guid.NewGuid() },
                new Unidade { Id = Guid.NewGuid(), Nome = "Unidade 2", TenantId = Guid.NewGuid() },
                new Unidade { Id = Guid.NewGuid(), Nome = "Unidade 3", TenantId = Guid.NewGuid() }
            };

            _repositoryMock
                .Setup(r => r.GetAllAdminAsync())
                .ReturnsAsync(unidadesEsperadas);

            // Act
            var result = await _service.GetAllUnidadesAdminAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().BeEquivalentTo(unidadesEsperadas);
            _repositoryMock.Verify(r => r.GetAllAdminAsync(), Times.Once);
        }

        [Fact]
        public async Task GetUnidadeByIdAsync_IdValido_DeveRetornarUnidade()
        {
            // Arrange
            var unidadeId = Guid.NewGuid();
            var tenantId = Guid.NewGuid();
            var unidadeEsperada = new Unidade 
            { 
                Id = unidadeId, 
                Nome = "Unidade Teste", 
                TenantId = tenantId 
            };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(unidadeId, tenantId))
                .ReturnsAsync(unidadeEsperada);

            // Act
            var result = await _service.GetUnidadeByIdAsync(unidadeId, tenantId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(unidadeId);
            result.Nome.Should().Be("Unidade Teste");
            _repositoryMock.Verify(r => r.GetByIdAsync(unidadeId, tenantId), Times.Once);
        }

        [Fact]
        public async Task GetUnidadeByIdAsync_IdVazio_DeveLancarExcecao()
        {
            // Arrange
            var tenantId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                _service.GetUnidadeByIdAsync(Guid.Empty, tenantId));
        }

        [Fact]
        public async Task UpdateUnidadeAsync_DadosValidos_DeveAtualizarUnidade()
        {
            // Arrange
            var unidadeId = Guid.NewGuid();
            var tenantId = Guid.NewGuid();
            var dto = new UpdateUnidadeDto
            {
                Nome = "Unidade Atualizada",
                Descricao = "Descrição atualizada",
                Endereco = "Endereço atualizado",
                MetaMensal = 75000,
                DataInicio = DateTime.UtcNow.AddDays(-30)
            };

            var unidadeExistente = new Unidade
            {
                Id = unidadeId,
                Nome = "Unidade Antiga",
                MetaMensal = 50000,
                TenantId = tenantId
            };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(unidadeId, tenantId))
                .ReturnsAsync(unidadeExistente);

            // Act
            var result = await _service.UpdateUnidadeAsync(unidadeId, dto, tenantId);

            // Assert
            result.Should().BeTrue();
            unidadeExistente.Nome.Should().Be(dto.Nome);
            unidadeExistente.MetaMensal.Should().Be(dto.MetaMensal);
            _repositoryMock.Verify(r => r.Update(unidadeExistente), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateUnidadeAsync_UnidadeNaoEncontrada_DeveRetornarFalso()
        {
            // Arrange
            var unidadeId = Guid.NewGuid();
            var tenantId = Guid.NewGuid();
            var dto = new UpdateUnidadeDto
            {
                Nome = "Unidade Atualizada",
                MetaMensal = 75000,
                DataInicio = DateTime.UtcNow
            };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(unidadeId, tenantId))
                .ReturnsAsync((Unidade?)null);

            // Act
            var result = await _service.UpdateUnidadeAsync(unidadeId, dto, tenantId);

            // Assert
            result.Should().BeFalse();
            _repositoryMock.Verify(r => r.Update(It.IsAny<Unidade>()), Times.Never);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task DeactivateUnidadeAsync_UnidadeAtiva_DeveDesativar()
        {
            // Arrange
            var unidadeId = Guid.NewGuid();
            var tenantId = Guid.NewGuid();
            var unidadeExistente = new Unidade 
            { 
                Id = unidadeId, 
                TenantId = tenantId,
                IsAtiva = true 
            };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(unidadeId, tenantId))
                .ReturnsAsync(unidadeExistente);

            // Act
            var result = await _service.DeactivateUnidadeAsync(unidadeId, tenantId);

            // Assert
            result.Should().BeTrue();
            unidadeExistente.IsAtiva.Should().BeFalse();
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeactivateUnidadeAsync_UnidadeJaDesativada_DeveRetornarVerdadeiro()
        {
            // Arrange
            var unidadeId = Guid.NewGuid();
            var tenantId = Guid.NewGuid();
            var unidadeExistente = new Unidade 
            { 
                Id = unidadeId, 
                TenantId = tenantId,
                IsAtiva = false 
            };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(unidadeId, tenantId))
                .ReturnsAsync(unidadeExistente);

            // Act
            var result = await _service.DeactivateUnidadeAsync(unidadeId, tenantId);

            // Assert
            result.Should().BeTrue();
            unidadeExistente.IsAtiva.Should().BeFalse();
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task DeleteUnidadeAsync_IdValido_DeveRemoverUnidade()
        {
            // Arrange
            var unidadeId = Guid.NewGuid();
            var tenantId = Guid.NewGuid();
            var unidadeExistente = new Unidade 
            { 
                Id = unidadeId, 
                TenantId = tenantId 
            };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(unidadeId, tenantId))
                .ReturnsAsync(unidadeExistente);

            // Act
            var result = await _service.DeleteUnidadeAsync(unidadeId, tenantId);

            // Assert
            result.Should().BeTrue();
            _repositoryMock.Verify(r => r.Remove(unidadeExistente), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateProjecaoAsync_ProjecaoValida_DeveAtualizar()
        {
            // Arrange
            var unidadeId = Guid.NewGuid();
            var projecao = 15000.50m;

            _repositoryMock
                .Setup(r => r.UpdateProjecaoAsync(unidadeId, projecao))
                .ReturnsAsync(true);

            // Act
            var result = await _service.UpdateProjecaoAsync(unidadeId, projecao);

            // Assert
            result.Should().BeTrue();
            _repositoryMock.Verify(r => r.UpdateProjecaoAsync(unidadeId, projecao), Times.Once);
        }

        [Fact]
        public async Task UpdateProjecaoAsync_ProjecaoNegativa_DeveLancarExcecao()
        {
            // Arrange
            var unidadeId = Guid.NewGuid();
            var projecao = -1000m;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                _service.UpdateProjecaoAsync(unidadeId, projecao));
        }

        [Fact]
        public async Task CreateUnidadeAsync_DTONulo_DeveLancarArgumentNullException()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
                _service.CreateUnidadeAsync(null!, userId, tenantId));
        }
    }
}