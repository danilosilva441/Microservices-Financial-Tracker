using BillingService.DTOs;
using BillingService.Models;
using BillingService.Repositories.Interfaces;
using BillingService.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SharedKernel;
using SharedKernel.Exceptions;
using Xunit;

namespace BillingService.Tests.Services
{
    public class MetaServiceTests
    {
        private readonly Mock<IMetaRepository> _repositoryMock;
        private readonly Mock<IUnidadeRepository> _unidadeRepoMock;
        private readonly Mock<ILogger<MetaService>> _loggerMock; // ✅ ADICIONAR
        private readonly MetaService _service;

        private readonly Guid _unidadeId = Guid.NewGuid();
        private readonly Guid _tenantId = Guid.NewGuid();
        private readonly Unidade _unidadeValida;

        public MetaServiceTests()
        {
            _repositoryMock = new Mock<IMetaRepository>();
            _unidadeRepoMock = new Mock<IUnidadeRepository>();
            _loggerMock = new Mock<ILogger<MetaService>>(); // ✅ ADICIONAR
            
            // ✅ CORRIGIR: Adicionar o loggerMock no construtor
            _service = new MetaService(
                _repositoryMock.Object, 
                _unidadeRepoMock.Object, 
                _loggerMock.Object); // ✅ ADICIONAR
            
            _unidadeValida = new Unidade { Id = _unidadeId, Nome = "Unidade Teste" };
        }

        public class SetMetaAsyncTests : MetaServiceTests
        {
            [Fact]
            public async Task QuandoUnidadeNaoExiste_DeveRetornarErro()
            {
                // Arrange
                var dto = CreateValidMetaDto();

                _unidadeRepoMock.Setup(r => r.GetByIdAsync(_unidadeId, _tenantId))
                    .ReturnsAsync((Unidade?)null);

                // Act
                var (result, error) = await _service.SetMetaAsync(_unidadeId, dto, _tenantId);

                // Assert
                result.Should().BeNull();
                error.Should().NotBeNullOrEmpty();
                
                _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Meta>()), Times.Never);
                _repositoryMock.Verify(r => r.Update(It.IsAny<Meta>()), Times.Never);
            }

            [Fact]
            public async Task QuandoMetaNaoExiste_DeveCriarNovaMeta()
            {
                // Arrange
                var dto = CreateValidMetaDto();

                _unidadeRepoMock.Setup(r => r.GetByIdAsync(_unidadeId, _tenantId))
                    .ReturnsAsync(_unidadeValida);
                
                _repositoryMock.Setup(r => r.GetByUnidadeAndPeriodAsync(_unidadeId, dto.Mes, dto.Ano, _tenantId))
                    .ReturnsAsync((Meta?)null);

                Meta? metaCriada = null;
                _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Meta>()))
                    .Callback<Meta>(meta => metaCriada = meta)
                    .Returns(Task.CompletedTask);

                // Act
                var (result, error) = await _service.SetMetaAsync(_unidadeId, dto, _tenantId);

                // Assert
                error.Should().BeNull();
                result.Should().NotBeNull();
                result!.ValorAlvo.Should().Be(dto.ValorAlvo);
                result.Mes.Should().Be(dto.Mes);
                result.Ano.Should().Be(dto.Ano);
                
                metaCriada.Should().NotBeNull();
                metaCriada!.UnidadeId.Should().Be(_unidadeId);
                metaCriada.TenantId.Should().Be(_tenantId);
                metaCriada.ValorAlvo.Should().Be(dto.ValorAlvo);

                _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Meta>()), Times.Once);
                _repositoryMock.Verify(r => r.Update(It.IsAny<Meta>()), Times.Never);
                _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
            }

            [Fact]
            public async Task QuandoMetaExiste_DeveAtualizarValorExistente()
            {
                // Arrange
                var dto = CreateValidMetaDto();
                var metaExistente = CreateMetaExistente();

                _unidadeRepoMock.Setup(r => r.GetByIdAsync(_unidadeId, _tenantId))
                    .ReturnsAsync(_unidadeValida);
                
                _repositoryMock.Setup(r => r.GetByUnidadeAndPeriodAsync(_unidadeId, dto.Mes, dto.Ano, _tenantId))
                    .ReturnsAsync(metaExistente);

                // Act
                var (result, error) = await _service.SetMetaAsync(_unidadeId, dto, _tenantId);

                // Assert
                error.Should().BeNull();
                result.Should().NotBeNull();
                result!.ValorAlvo.Should().Be(dto.ValorAlvo);
                result.Id.Should().Be(metaExistente.Id);

                _repositoryMock.Verify(r => r.Update(metaExistente), Times.Once);
                _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Meta>()), Times.Never);
                _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
            }

            [Theory]
            [InlineData(0)]
            [InlineData(-100)]
            [InlineData(-0.01)]
            public async Task QuandoValorAlvoInvalido_DeveRetornarErro(decimal valorInvalido)
            {
                // Arrange
                var dto = CreateValidMetaDto();
                dto.ValorAlvo = valorInvalido;

                _unidadeRepoMock.Setup(r => r.GetByIdAsync(_unidadeId, _tenantId))
                    .ReturnsAsync(_unidadeValida);

                // Act
                var (result, error) = await _service.SetMetaAsync(_unidadeId, dto, _tenantId);

                // Assert
                result.Should().BeNull();
                error.Should().NotBeNullOrEmpty();
                
                _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Meta>()), Times.Never);
                _repositoryMock.Verify(r => r.Update(It.IsAny<Meta>()), Times.Never);
            }

            [Theory]
            [InlineData(0)]
            [InlineData(13)]
            [InlineData(-1)]
            public async Task QuandoMesInvalido_DeveRetornarErro(int mesInvalido)
            {
                // Arrange
                var dto = CreateValidMetaDto();
                dto.Mes = mesInvalido;

                _unidadeRepoMock.Setup(r => r.GetByIdAsync(_unidadeId, _tenantId))
                    .ReturnsAsync(_unidadeValida);

                // Act
                var (result, error) = await _service.SetMetaAsync(_unidadeId, dto, _tenantId);

                // Assert
                result.Should().BeNull();
                error.Should().NotBeNullOrEmpty();
            }

            [Theory]
            [InlineData(1999)]
            [InlineData(2031)]
            [InlineData(1899)]
            public async Task QuandoAnoInvalido_DeveRetornarErro(int anoInvalido)
            {
                // Arrange
                var dto = CreateValidMetaDto();
                dto.Ano = anoInvalido;

                _unidadeRepoMock.Setup(r => r.GetByIdAsync(_unidadeId, _tenantId))
                    .ReturnsAsync(_unidadeValida);

                // Act
                var (result, error) = await _service.SetMetaAsync(_unidadeId, dto, _tenantId);

                // Assert
                result.Should().BeNull();
                error.Should().NotBeNullOrEmpty();
            }
        }

        public class GetMetaAsyncTests : MetaServiceTests
        {
            [Fact]
            public async Task QuandoMetaExiste_DeveRetornarMeta()
            {
                // Arrange
                var mes = 11;
                var ano = 2025;
                var metaExistente = CreateMetaExistente();

                _repositoryMock.Setup(r => r.GetByUnidadeAndPeriodAsync(_unidadeId, mes, ano, _tenantId))
                    .ReturnsAsync(metaExistente);

                // Act
                var result = await _service.GetMetaAsync(_unidadeId, mes, ano, _tenantId);

                // Assert
                result.Should().NotBeNull();
                result!.ValorAlvo.Should().Be(metaExistente.ValorAlvo);
            }

            [Fact]
            public async Task QuandoMetaNaoExiste_DeveRetornarNull()
            {
                // Arrange
                var mes = 11;
                var ano = 2025;

                _repositoryMock.Setup(r => r.GetByUnidadeAndPeriodAsync(_unidadeId, mes, ano, _tenantId))
                    .ReturnsAsync((Meta?)null);

                // Act
                var result = await _service.GetMetaAsync(_unidadeId, mes, ano, _tenantId);

                // Assert
                result.Should().BeNull();
            }
        }

        // Helper methods
        private MetaDto CreateValidMetaDto()
        {
            return new MetaDto 
            { 
                Mes = 11, 
                Ano = 2025, 
                ValorAlvo = 60000 
            };
        }

        private Meta CreateMetaExistente()
        {
            return new Meta 
            { 
                Id = Guid.NewGuid(),
                UnidadeId = _unidadeId,
                TenantId = _tenantId,
                Mes = 11,
                Ano = 2025,
                ValorAlvo = 50000
            };
        }
    }
}