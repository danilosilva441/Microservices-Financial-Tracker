using BillingService.Data;
using BillingService.DTOs;
using BillingService.Models;
using BillingService.Repositories.Interfaces;
using BillingService.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SharedKernel;
using SharedKernel.Exceptions;
using Xunit;

namespace BillingService.Tests.Services
{
    public class FaturamentoParcialServiceTests : IDisposable
    {
        private readonly Mock<IFaturamentoParcialRepository> _repositoryMock;
        private readonly Mock<IUnidadeRepository> _unidadeRepositoryMock;
        private readonly Mock<ILogger<FaturamentoParcialService>> _loggerMock;
        private readonly BillingDbContext _context;
        private readonly FaturamentoParcialService _service;

        private readonly Guid _tenantId = Guid.NewGuid();
        private readonly Guid _userId = Guid.NewGuid();
        private readonly Guid _unidadeId = Guid.NewGuid();
        private readonly Guid _metodoPagamentoId = Guid.NewGuid();

        public FaturamentoParcialServiceTests()
        {
            _repositoryMock = new Mock<IFaturamentoParcialRepository>();
            _unidadeRepositoryMock = new Mock<IUnidadeRepository>();
            _loggerMock = new Mock<ILogger<FaturamentoParcialService>>();

            var options = new DbContextOptionsBuilder<BillingDbContext>()
                .UseInMemoryDatabase(databaseName: $"BillingService_FaturamentoParcial_{Guid.NewGuid()}")
                .Options;

            _context = new BillingDbContext(options, null);

            _service = new FaturamentoParcialService(
                _repositoryMock.Object,
                _unidadeRepositoryMock.Object,
                _context,
                _loggerMock.Object);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

        public class AddFaturamentoAsyncTests : FaturamentoParcialServiceTests
        {
            [Fact]
            public async Task QuandoUsuarioSemAcessoAUnidade_DeveLancarExcecao()
            {
                // Arrange
                var dto = CreateValidFaturamentoParcialDto();

                _repositoryMock
                    .Setup(r => r.UserHasAccessToUnidadeAsync(_unidadeId, _userId, _tenantId))
                    .ReturnsAsync(false);

                // Act & Assert
                await Assert.ThrowsAsync<UnidadeAccessDeniedException>(() =>
                    _service.AddFaturamentoAsync(_unidadeId, dto, _userId, _tenantId));

                _repositoryMock.Verify(r => r.UserHasAccessToUnidadeAsync(_unidadeId, _userId, _tenantId), Times.Once);
                _repositoryMock.Verify(r => r.AddAsync(It.IsAny<FaturamentoParcial>()), Times.Never);
            }

            [Fact]
            public async Task QuandoDadosValidos_DeveCriarFaturamentoParcialCorretamente()
            {
                // Arrange
                var dto = CreateValidFaturamentoParcialDto();
                var faturamentoDiario = CreateFaturamentoDiario();

                _repositoryMock
                    .Setup(r => r.UserHasAccessToUnidadeAsync(_unidadeId, _userId, _tenantId))
                    .ReturnsAsync(true);

                _repositoryMock
                    .Setup(r => r.CheckForOverlappingFaturamentoAsync(
                        It.IsAny<Guid>(), _tenantId, dto.HoraInicio, dto.HoraFim, null))
                    .ReturnsAsync(false);

                // Configurar o DbContext para retornar o faturamento diário
                var options = new DbContextOptionsBuilder<BillingDbContext>()
                    .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                    .Options;

                using var context = new BillingDbContext(options, null);
                context.FaturamentosDiarios.Add(faturamentoDiario);
                await context.SaveChangesAsync();

                var service = new FaturamentoParcialService(
                    _repositoryMock.Object,
                    _unidadeRepositoryMock.Object,
                    context,
                    _loggerMock.Object);

                // Act
                var result = await service.AddFaturamentoAsync(_unidadeId, dto, _userId, _tenantId);

                // Assert
                result.Should().NotBeNull();
                result.Valor.Should().Be(dto.Valor);
                result.HoraInicio.Should().BeCloseTo(dto.HoraInicio, TimeSpan.FromSeconds(1));
                result.HoraFim.Should().BeCloseTo(dto.HoraFim, TimeSpan.FromSeconds(1));
                result.MetodoPagamentoId.Should().Be(dto.MetodoPagamentoId);

                _repositoryMock.Verify(r => r.AddAsync(It.Is<FaturamentoParcial>(fp =>
                    fp.Valor == dto.Valor &&
                    fp.MetodoPagamentoId == dto.MetodoPagamentoId &&
                    fp.TenantId == _tenantId)),
                    Times.Once);
            }

            [Theory]
            [InlineData(0)]
            [InlineData(-100)]
            public async Task QuandoValorInvalido_DeveLancarExcecao(decimal valorInvalido)
            {
                // Arrange
                var dto = CreateValidFaturamentoParcialDto();
                dto.Valor = valorInvalido;

                _repositoryMock
                    .Setup(r => r.UserHasAccessToUnidadeAsync(_unidadeId, _userId, _tenantId))
                    .ReturnsAsync(true);

                // Act & Assert
                await Assert.ThrowsAsync<BusinessRuleException>(() =>
                    _service.AddFaturamentoAsync(_unidadeId, dto, _userId, _tenantId));

                _repositoryMock.Verify(r => r.AddAsync(It.IsAny<FaturamentoParcial>()), Times.Never);
            }

            [Fact]
            public async Task QuandoHoraFimAntesHoraInicio_DeveLancarExcecao()
            {
                // Arrange
                var dto = CreateValidFaturamentoParcialDto();
                dto.HoraInicio = DateTime.UtcNow.AddHours(2);
                dto.HoraFim = DateTime.UtcNow.AddHours(1);

                _repositoryMock
                    .Setup(r => r.UserHasAccessToUnidadeAsync(_unidadeId, _userId, _tenantId))
                    .ReturnsAsync(true);

                // Act & Assert
                await Assert.ThrowsAsync<InvalidFaturamentoTimeException>(() =>
                    _service.AddFaturamentoAsync(_unidadeId, dto, _userId, _tenantId));

                _repositoryMock.Verify(r => r.AddAsync(It.IsAny<FaturamentoParcial>()), Times.Never);
            }
        }

        public class GetFaturamentosPorUnidadeEDataTests : FaturamentoParcialServiceTests
        {
            [Fact]
            public async Task QuandoExistemFaturamentos_DeveRetornarLista()
            {
                // Arrange
                var data = DateOnly.FromDateTime(DateTime.UtcNow);
                var faturamentoDiario = CreateFaturamentoDiario();

                // Configurar dados no contexto em memória
                _context.FaturamentosDiarios.Add(faturamentoDiario);

                var faturamentos = new List<FaturamentoParcial>
                {
                    CreateFaturamentoParcial(Guid.NewGuid(), faturamentoDiario),
                    CreateFaturamentoParcial(Guid.NewGuid(), faturamentoDiario)
                };

                _context.FaturamentosParciais.AddRange(faturamentos);
                await _context.SaveChangesAsync();

                // Act
                var result = await _service.GetFaturamentosPorUnidadeEDataAsync(_unidadeId, data, _tenantId);

                // Assert
                result.Should().NotBeNull();
                result.Should().HaveCount(2);
            }

            [Fact]
            public async Task QuandoNaoExistemFaturamentos_DeveRetornarListaVazia()
            {
                // Arrange
                var data = DateOnly.FromDateTime(DateTime.UtcNow);

                // Act
                var result = await _service.GetFaturamentosPorUnidadeEDataAsync(_unidadeId, data, _tenantId);

                // Assert
                result.Should().NotBeNull();
                result.Should().BeEmpty();
            }
        }

        public class DeleteFaturamentoAsyncTests : FaturamentoParcialServiceTests
        {
            [Fact]
            public async Task QuandoFaturamentoExiste_DeveDeletarComSucesso()
            {
                // Arrange
                var faturamentoParcialId = Guid.NewGuid();
                var faturamentoDiario = CreateFaturamentoDiario();
                var faturamentoParcial = CreateFaturamentoParcial(faturamentoParcialId, faturamentoDiario);

                // 🔥 CORREÇÃO: Adicionar os dados ao contexto em memória
                _context.FaturamentosDiarios.Add(faturamentoDiario);
                _context.FaturamentosParciais.Add(faturamentoParcial);
                await _context.SaveChangesAsync();

                _repositoryMock
                    .Setup(r => r.GetByIdAsync(faturamentoParcialId, _tenantId))
                    .ReturnsAsync(faturamentoParcial);

                _repositoryMock
                    .Setup(r => r.UserHasAccessToUnidadeAsync(_unidadeId, _userId, _tenantId))
                    .ReturnsAsync(true);

                // Act
                await _service.DeleteFaturamentoAsync(_unidadeId, faturamentoParcialId, _userId, _tenantId);

                // Assert
                _repositoryMock.Verify(r => r.Remove(faturamentoParcial), Times.Once);
                _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
            }

            [Fact]
            public async Task QuandoFaturamentoNaoExiste_DeveLancarExcecao()
            {
                // Arrange
                var faturamentoParcialId = Guid.NewGuid();

                _repositoryMock
                    .Setup(r => r.GetByIdAsync(faturamentoParcialId, _tenantId))
                    .ReturnsAsync((FaturamentoParcial?)null);

                _repositoryMock
                    .Setup(r => r.UserHasAccessToUnidadeAsync(_unidadeId, _userId, _tenantId))
                    .ReturnsAsync(true);

                // Act & Assert
                await Assert.ThrowsAsync<FaturamentoParcialNotFoundException>(() =>
                    _service.DeleteFaturamentoAsync(_unidadeId, faturamentoParcialId, _userId, _tenantId));

                _repositoryMock.Verify(r => r.Remove(It.IsAny<FaturamentoParcial>()), Times.Never);
            }
        }

        // Helper methods
        private FaturamentoParcialCreateDto CreateValidFaturamentoParcialDto()
        {
            var agora = DateTime.UtcNow;
            return new FaturamentoParcialCreateDto
            {
                Valor = 150.75m,
                HoraInicio = agora.AddHours(-2),
                HoraFim = agora.AddHours(-1),
                MetodoPagamentoId = _metodoPagamentoId,
                Origem = "Teste"
            };
        }

        private FaturamentoDiario CreateFaturamentoDiario()
        {
            return new FaturamentoDiario
            {
                Id = Guid.NewGuid(),
                UnidadeId = _unidadeId,
                Data = DateOnly.FromDateTime(DateTime.UtcNow),
                TenantId = _tenantId,
                Status = RegistroStatus.Pendente,
                FundoDeCaixa = 0,
                Observacoes = "Teste"
            };
        }

        private FaturamentoParcial CreateFaturamentoParcial(
            Guid id,
            FaturamentoDiario faturamentoDiario,
            FaturamentoParcialCreateDto? dto = null)
        {
            dto ??= CreateValidFaturamentoParcialDto();

            return new FaturamentoParcial
            {
                Id = id,
                FaturamentoDiarioId = faturamentoDiario.Id,
                Valor = dto.Valor,
                HoraInicio = dto.HoraInicio,
                HoraFim = dto.HoraFim,
                MetodoPagamentoId = dto.MetodoPagamentoId,
                Origem = dto.Origem,
                TenantId = _tenantId,
                IsAtivo = true,
                FaturamentoDiario = faturamentoDiario
            };
        }
    }
}