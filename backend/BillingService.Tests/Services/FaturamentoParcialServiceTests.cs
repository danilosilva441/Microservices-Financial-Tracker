using BillingService.Data;
using BillingService.DTOs;
using BillingService.Models;
using BillingService.Repositories.Interfaces;
using BillingService.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using SharedKernel;
using Xunit;

namespace BillingService.Tests.Services
{
    public class FaturamentoParcialServiceTests
    {
        private readonly Mock<IFaturamentoParcialRepository> _repoMock;
        private readonly Mock<IUnidadeRepository> _unidadeRepoMock;
        private readonly BillingDbContext _context; 
        private readonly FaturamentoParcialService _service;

        public FaturamentoParcialServiceTests()
        {
            _repoMock = new Mock<IFaturamentoParcialRepository>();
            _unidadeRepoMock = new Mock<IUnidadeRepository>();

            var options = new DbContextOptionsBuilder<BillingDbContext>()
                .UseInMemoryDatabase(databaseName: $"BillingService_FatParcial_{Guid.NewGuid()}")
                .Options;
            
            _context = new BillingDbContext(options, null);

            _service = new FaturamentoParcialService(_repoMock.Object, _unidadeRepoMock.Object, _context);
        }

        [Fact]
        public async Task AddFaturamentoAsync_SeFaturamentoDiarioNaoExiste_DeveCriarNovo()
        {
            // Arrange
            var unidadeId = Guid.NewGuid();
            var tenantId = Guid.NewGuid();
            var dto = new FaturamentoParcialCreateDto 
            { 
                Valor = 100, 
                HoraInicio = DateTime.UtcNow, 
                HoraFim = DateTime.UtcNow.AddHours(1),
                MetodoPagamentoId = Guid.NewGuid()
            };

            _repoMock.Setup(r => r.UserHasAccessToUnidadeAsync(unidadeId, It.IsAny<Guid>(), tenantId))
                .ReturnsAsync(true);

            // Act
            await _service.AddFaturamentoAsync(unidadeId, dto, Guid.NewGuid(), tenantId);

            // Assert
            // CORREÇÃO: Verificamos .Local para ver o que foi adicionado ao contexto mas ainda não salvo (devido ao Mock)
            var diario = _context.FaturamentosDiarios.Local.FirstOrDefault();
            
            diario.Should().NotBeNull("o serviço deveria ter criado o cabeçalho diário automaticamente");
            diario!.UnidadeId.Should().Be(unidadeId);
            diario.TenantId.Should().Be(tenantId);
            
            _repoMock.Verify(r => r.AddAsync(It.IsAny<FaturamentoParcial>()), Times.Once);
        }
    }
}