using BillingService.DTOs;
using BillingService.Models;
using BillingService.Repositories.Interfaces;
using BillingService.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SharedKernel;
using Xunit;

namespace BillingService.Tests.Services
{
    public class FaturamentoDiarioServiceTests
    {
        private readonly Mock<IFaturamentoDiarioRepository> _repositoryMock;
        private readonly Mock<IUnidadeRepository> _unidadeRepositoryMock;
        private readonly Mock<ILogger<FaturamentoDiarioService>> _loggerMock;
        private readonly FaturamentoDiarioService _service;

        private readonly Guid _tenantId = Guid.NewGuid();
        private readonly Guid _userId = Guid.NewGuid();

        public FaturamentoDiarioServiceTests()
        {
            _repositoryMock = new Mock<IFaturamentoDiarioRepository>();
            _unidadeRepositoryMock = new Mock<IUnidadeRepository>();
            _loggerMock = new Mock<ILogger<FaturamentoDiarioService>>();
            _service = new FaturamentoDiarioService(
                _repositoryMock.Object, 
                _unidadeRepositoryMock.Object,
                _loggerMock.Object);
        }

        public class GetFechamentosPendentesAsyncTests : FaturamentoDiarioServiceTests
        {
            [Fact]
            public async Task QuandoExistemFechamentosPendentes_DeveRetornarListaComValoresCalculados()
            {
                // Arrange
                var fechamentos = new List<FaturamentoDiario>
                {
                    CreateFaturamentoDiario(RegistroStatus.Pendente, 100.00m, 50.50m),
                    CreateFaturamentoDiario(RegistroStatus.Pendente, 75.25m, 25.75m)
                };

                _repositoryMock
                    .Setup(r => r.ListByStatusAsync(RegistroStatus.Pendente, _tenantId))
                    .ReturnsAsync(fechamentos);

                // Act
                var result = (await _service.GetFechamentosPendentesAsync(_tenantId)).ToList();

                // Assert
                result.Should().NotBeNull();
                result.Should().HaveCount(2);
                
                result[0].ValorTotalParciais.Should().Be(150.50m);
                result[1].ValorTotalParciais.Should().Be(101.00m);
                
                _repositoryMock.Verify(r => r.ListByStatusAsync(RegistroStatus.Pendente, _tenantId), Times.Once);
            }

            [Fact]
            public async Task QuandoNaoExistemFechamentosPendentes_DeveRetornarListaVazia()
            {
                // Arrange
                _repositoryMock
                    .Setup(r => r.ListByStatusAsync(RegistroStatus.Pendente, _tenantId))
                    .ReturnsAsync(new List<FaturamentoDiario>());

                // Act
                var result = await _service.GetFechamentosPendentesAsync(_tenantId);

                // Assert
                result.Should().NotBeNull();
                result.Should().BeEmpty();
            }

            [Fact]
            public async Task QuandoFechamentoPossuiListaVaziaDeParciais_DeveRetornarZero()
            {
                // Arrange
                var fechamentos = new List<FaturamentoDiario>
                {
                    CreateFaturamentoDiario(RegistroStatus.Pendente) // Sem faturamentos parciais
                };

                _repositoryMock
                    .Setup(r => r.ListByStatusAsync(RegistroStatus.Pendente, _tenantId))
                    .ReturnsAsync(fechamentos);

                // Act
                var result = (await _service.GetFechamentosPendentesAsync(_tenantId)).ToList();

                // Assert
                result.Should().HaveCount(1);
                result[0].ValorTotalParciais.Should().Be(0);
            }

            [Fact]
            public async Task QuandoFechamentoPossuiParciaisNulos_DeveRetornarZero()
            {
                // Arrange
                var fechamento = CreateFaturamentoDiario(RegistroStatus.Pendente);
                fechamento.FaturamentosParciais = null; // Caso de null

                _repositoryMock
                    .Setup(r => r.ListByStatusAsync(RegistroStatus.Pendente, _tenantId))
                    .ReturnsAsync(new List<FaturamentoDiario> { fechamento });

                // Act
                var result = (await _service.GetFechamentosPendentesAsync(_tenantId)).ToList();

                // Assert
                result[0].ValorTotalParciais.Should().Be(0);
            }
        }

        public class RevisarFechamentoAsyncTests : FaturamentoDiarioServiceTests
        {
            [Theory]
            [InlineData(RegistroStatus.Aprovado, RegistroStatus.Pendente)]
            public async Task QuandoTentaTransicaoDeStatusInvalida_DeveRetornarErro(
                RegistroStatus statusAtual, 
                RegistroStatus novoStatus)
            {
                // Arrange
                var fechamentoId = Guid.NewGuid();
                var fechamentoExistente = CreateFaturamentoDiario(statusAtual);
                fechamentoExistente.Id = fechamentoId;

                var dto = new FaturamentoDiarioSupervisorUpdateDto 
                { 
                    Status = novoStatus,
                    FundoDeCaixa = 1000.50m,
                    Observacoes = "Observação teste",
                    ValorAtm = 500.00m,
                    ValorBoletosMensalistas = 300.00m
                };

                _repositoryMock
                    .Setup(r => r.GetByIdAsync(fechamentoId, _tenantId))
                    .ReturnsAsync(fechamentoExistente);

                // Act
                var (result, error) = await _service.RevisarFechamentoAsync(fechamentoId, dto, _userId, _tenantId);

                // Assert
                result.Should().BeNull();
                error.Should().Be(ErrorMessages.NoAlteredStatus);
                
                _repositoryMock.Verify(r => r.Update(It.IsAny<FaturamentoDiario>()), Times.Never);
                _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
            }

            [Theory]
            [InlineData(RegistroStatus.Pendente, RegistroStatus.Aprovado)]
            [InlineData(RegistroStatus.Pendente, RegistroStatus.Rejeitado)]
            public async Task QuandoTransicaoDeStatusValida_DeveAtualizarFechamento(
                RegistroStatus statusAtual, 
                RegistroStatus novoStatus)
            {
                // Arrange
                var fechamentoId = Guid.NewGuid();
                var fechamentoExistente = CreateFaturamentoDiario(statusAtual);
                fechamentoExistente.Id = fechamentoId;

                var dto = new FaturamentoDiarioSupervisorUpdateDto 
                { 
                    Status = novoStatus,
                    FundoDeCaixa = 1000.50m,
                    Observacoes = "Teste observação",
                    ValorAtm = 500.00m,
                    ValorBoletosMensalistas = 300.00m
                };

                _repositoryMock
                    .Setup(r => r.GetByIdAsync(fechamentoId, _tenantId))
                    .ReturnsAsync(fechamentoExistente);

                // Act
                var (result, error) = await _service.RevisarFechamentoAsync(fechamentoId, dto, _userId, _tenantId);

                // Assert
                error.Should().BeNull();
                result.Should().NotBeNull();
                result!.Status.Should().Be(novoStatus.ToString());
                result.Observacoes.Should().Be(dto.Observacoes);
                result.FundoDeCaixa.Should().Be(dto.FundoDeCaixa);
                result.ValorAtm.Should().Be(dto.ValorAtm);
                result.ValorBoletosMensalistas.Should().Be(dto.ValorBoletosMensalistas);
                
                _repositoryMock.Verify(r => r.Update(It.Is<FaturamentoDiario>(f => 
                    f.Id == fechamentoId && 
                    f.Status == novoStatus)), 
                    Times.Once);
                _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
            }

            [Fact]
            public async Task QuandoFechamentoNaoExiste_DeveRetornarErro()
            {
                // Arrange
                var fechamentoId = Guid.NewGuid();
                var dto = new FaturamentoDiarioSupervisorUpdateDto 
                { 
                    Status = RegistroStatus.Aprovado,
                    FundoDeCaixa = 1000.50m
                };

                _repositoryMock
                    .Setup(r => r.GetByIdAsync(fechamentoId, _tenantId))
                    .ReturnsAsync((FaturamentoDiario?)null);

                // Act
                var (result, error) = await _service.RevisarFechamentoAsync(fechamentoId, dto, _userId, _tenantId);

                // Assert
                result.Should().BeNull();
                error.Should().Be(ErrorMessages.FechamentoNotFound);
            }
        }

        public class SubmeterFechamentoAsyncTests : FaturamentoDiarioServiceTests
        {
            [Fact]
            public async Task QuandoUnidadeNaoExiste_DeveRetornarErro()
            {
                // Arrange
                var unidadeId = Guid.NewGuid();
                var dto = new FaturamentoDiarioCreateDto
                {
                    Data = DateOnly.FromDateTime(DateTime.UtcNow.Date),
                    FundoDeCaixa = 1000.00m,
                    Observacoes = "Teste"
                };

                _unidadeRepositoryMock
                    .Setup(r => r.GetByIdAsync(unidadeId, _tenantId))
                    .ReturnsAsync((Unidade?)null);

                // Act
                var (result, error) = await _service.SubmeterFechamentoAsync(unidadeId, dto, _userId, _tenantId);

                // Assert
                result.Should().BeNull();
                error.Should().Be(ErrorMessages.UnidadeNotFound);
            }

            [Fact]
            public async Task QuandoFechamentoJaExiste_DeveRetornarErro()
            {
                // Arrange
                var unidadeId = Guid.NewGuid();
                var data = DateOnly.FromDateTime(DateTime.UtcNow.Date);
                var dto = new FaturamentoDiarioCreateDto
                {
                    Data = data,
                    FundoDeCaixa = 1000.00m,
                    Observacoes = "Teste"
                };

                var unidade = new Unidade { Id = unidadeId, Nome = "Unidade Teste" };
                var fechamentoExistente = CreateFaturamentoDiario(RegistroStatus.Pendente);

                _unidadeRepositoryMock
                    .Setup(r => r.GetByIdAsync(unidadeId, _tenantId))
                    .ReturnsAsync(unidade);

                _repositoryMock
                    .Setup(r => r.GetByUnidadeAndDateAsync(unidadeId, data, _tenantId))
                    .ReturnsAsync(fechamentoExistente);

                // Act
                var (result, error) = await _service.SubmeterFechamentoAsync(unidadeId, dto, _userId, _tenantId);

                // Assert
                result.Should().BeNull();
                error.Should().Be(ErrorMessages.FechamentoJaExiste);
            }

            [Fact]
            public async Task QuandoDadosValidos_DeveCriarFechamento()
            {
                // Arrange
                var unidadeId = Guid.NewGuid();
                var data = DateOnly.FromDateTime(DateTime.UtcNow.Date);
                var dto = new FaturamentoDiarioCreateDto
                {
                    Data = data,
                    FundoDeCaixa = 1000.00m,
                    Observacoes = "Teste observação"
                };

                var unidade = new Unidade { Id = unidadeId, Nome = "Unidade Teste" };

                _unidadeRepositoryMock
                    .Setup(r => r.GetByIdAsync(unidadeId, _tenantId))
                    .ReturnsAsync(unidade);

                _repositoryMock
                    .Setup(r => r.GetByUnidadeAndDateAsync(unidadeId, data, _tenantId))
                    .ReturnsAsync((FaturamentoDiario?)null);

                // Act
                var (result, error) = await _service.SubmeterFechamentoAsync(unidadeId, dto, _userId, _tenantId);

                // Assert
                error.Should().BeNull();
                result.Should().NotBeNull();
                result!.UnidadeId.Should().Be(unidadeId);
                result.Data.Should().Be(data);
                result.Status.Should().Be(RegistroStatus.Pendente.ToString());
                result.FundoDeCaixa.Should().Be(dto.FundoDeCaixa);
                result.Observacoes.Should().Be(dto.Observacoes);
                
                _repositoryMock.Verify(r => r.AddAsync(It.Is<FaturamentoDiario>(f => 
                    f.UnidadeId == unidadeId && 
                    f.Data == data &&
                    f.Status == RegistroStatus.Pendente)), 
                    Times.Once);
                _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
            }

            [Fact]
            public async Task QuandoDataFutura_DeveRetornarErro()
            {
                // Arrange
                var unidadeId = Guid.NewGuid();
                var dataFutura = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1).Date);
                var dto = new FaturamentoDiarioCreateDto
                {
                    Data = dataFutura,
                    FundoDeCaixa = 1000.00m,
                    Observacoes = "Teste"
                };

                var unidade = new Unidade { Id = unidadeId, Nome = "Unidade Teste" };

                _unidadeRepositoryMock
                    .Setup(r => r.GetByIdAsync(unidadeId, _tenantId))
                    .ReturnsAsync(unidade);

                // Act
                var (result, error) = await _service.SubmeterFechamentoAsync(unidadeId, dto, _userId, _tenantId);

                // Assert
                result.Should().BeNull();
                error.Should().Be("Data não pode ser futura");
            }

            [Fact]
            public async Task QuandoFundoDeCaixaNegativo_DeveRetornarErro()
            {
                // Arrange
                var unidadeId = Guid.NewGuid();
                var dto = new FaturamentoDiarioCreateDto
                {
                    Data = DateOnly.FromDateTime(DateTime.UtcNow.Date),
                    FundoDeCaixa = -100.00m,
                    Observacoes = "Teste"
                };

                var unidade = new Unidade { Id = unidadeId, Nome = "Unidade Teste" };

                _unidadeRepositoryMock
                    .Setup(r => r.GetByIdAsync(unidadeId, _tenantId))
                    .ReturnsAsync(unidade);

                // Act
                var (result, error) = await _service.SubmeterFechamentoAsync(unidadeId, dto, _userId, _tenantId);

                // Assert
                result.Should().BeNull();
                error.Should().Be("Fundo de caixa não pode ser negativo");
            }
        }

        public class ExcecoesTests : FaturamentoDiarioServiceTests
        {
            [Fact]
            public async Task QuandoRepositoryLancaExcecao_DevePropagarExcecao()
            {
                // Arrange
                var exceptionMessage = "Database connection failed";
                
                _repositoryMock
                    .Setup(r => r.ListByStatusAsync(It.IsAny<RegistroStatus>(), _tenantId))
                    .ThrowsAsync(new InvalidOperationException(exceptionMessage));

                // Act
                Func<Task> action = async () => await _service.GetFechamentosPendentesAsync(_tenantId);

                // Assert
                await action.Should()
                    .ThrowAsync<FaturamentoServiceException>()
                    .WithMessage("Erro ao buscar fechamentos pendentes");
            }
        }

        // Helper methods para criar dados de teste
        private FaturamentoDiario CreateFaturamentoDiario(
            RegistroStatus status, 
            params decimal[] valoresParciais)
        {
            var faturamento = new FaturamentoDiario
            {
                Id = Guid.NewGuid(),
                Status = status,
                Data = DateOnly.FromDateTime(DateTime.UtcNow.Date),
                UnidadeId = Guid.NewGuid(),
                TenantId = _tenantId,
                FaturamentosParciais = new List<FaturamentoParcial>()
            };

            foreach (var valor in valoresParciais)
            {
                faturamento.FaturamentosParciais.Add(new FaturamentoParcial 
                { 
                    Id = Guid.NewGuid(),
                    Valor = valor 
                });
            }

            return faturamento;
        }
    }
}