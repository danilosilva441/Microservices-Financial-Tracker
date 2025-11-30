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
using Xunit;

namespace BillingService.Tests.Services
{
    public class ExpenseServiceTests : IDisposable
    {
        private readonly Mock<IExpenseRepository> _repositoryMock;
        private readonly Mock<IUnidadeRepository> _unidadeRepoMock;
        private readonly Mock<ILogger<ExpenseService>> _loggerMock;
        private readonly BillingDbContext _context;
        private readonly ExpenseService _service;

        private readonly Guid _tenantId = Guid.NewGuid();
        private readonly Guid _unidadeId = Guid.NewGuid();
        private readonly Guid _categoriaId = Guid.NewGuid();

        public ExpenseServiceTests()
        {
            _repositoryMock = new Mock<IExpenseRepository>();
            _unidadeRepoMock = new Mock<IUnidadeRepository>();
            _loggerMock = new Mock<ILogger<ExpenseService>>();

            // Configura DbContext em Memória
            var options = new DbContextOptionsBuilder<BillingDbContext>()
                .UseInMemoryDatabase(databaseName: $"BillingService_Expense_{Guid.NewGuid()}")
                .Options;
            
            _context = new BillingDbContext(options, null);

            // CORREÇÃO: Construtor com 4 argumentos (incluindo ILogger)
            _service = new ExpenseService(
                _repositoryMock.Object, 
                _unidadeRepoMock.Object, 
                _context,
                _loggerMock.Object);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

        public class CreateExpenseAsyncTests : ExpenseServiceTests
        {
            [Fact]
            public async Task QuandoUnidadeNaoExiste_DeveRetornarErroUnidadeNotFound()
            {
                // Arrange
                var dto = CreateValidExpenseDto();
                
                _unidadeRepoMock
                    .Setup(r => r.GetByIdAsync(_unidadeId, _tenantId))
                    .ReturnsAsync((Unidade?)null);

                // Act
                var (result, error) = await _service.CreateExpenseAsync(dto, _tenantId);

                // Assert
                result.Should().BeNull();
                error.Should().Be(ErrorMessages.UnidadeNotFound);
                
                _unidadeRepoMock.Verify(r => r.GetByIdAsync(_unidadeId, _tenantId), Times.Once);
                _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Expense>()), Times.Never);
            }

            [Fact]
            public async Task QuandoCategoriaNaoExiste_DeveRetornarErroCategoriaNotFound()
            {
                // Arrange
                var dto = CreateValidExpenseDto();
                var unidade = CreateUnidade();
                
                _unidadeRepoMock
                    .Setup(r => r.GetByIdAsync(_unidadeId, _tenantId))
                    .ReturnsAsync(unidade);
                
                _repositoryMock
                    .Setup(r => r.GetCategoryByIdAsync(_categoriaId, _tenantId))
                    .ReturnsAsync((ExpenseCategory?)null);

                // Act
                var (result, error) = await _service.CreateExpenseAsync(dto, _tenantId);

                // Assert
                result.Should().BeNull();
                error.Should().Be(ErrorMessages.CategoriaNotFound);
                
                _repositoryMock.Verify(r => r.GetCategoryByIdAsync(_categoriaId, _tenantId), Times.Once);
                _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Expense>()), Times.Never);
            }

            [Fact]
            public async Task QuandoDadosValidos_DeveCriarExpenseComSucesso()
            {
                // Arrange
                var dto = CreateValidExpenseDto();
                var unidade = CreateUnidade();
                var categoria = CreateCategoria();
                
                _unidadeRepoMock
                    .Setup(r => r.GetByIdAsync(_unidadeId, _tenantId))
                    .ReturnsAsync(unidade);
                
                _repositoryMock
                    .Setup(r => r.GetCategoryByIdAsync(_categoriaId, _tenantId))
                    .ReturnsAsync(categoria);

                var expenseId = Guid.NewGuid();
                var expenseCriado = CreateExpense(expenseId, unidade, categoria, dto);
                
                _repositoryMock
                    .Setup(r => r.GetByIdAsync(expenseId, _tenantId))
                    .ReturnsAsync(expenseCriado);

                // Configurar o AddAsync para retornar a expense criada
                _repositoryMock
                    .Setup(r => r.AddAsync(It.IsAny<Expense>()))
                    .Returns(Task.CompletedTask)
                    .Callback<Expense>(expense => expense.Id = expenseId);

                // Act
                var (result, error) = await _service.CreateExpenseAsync(dto, _tenantId);

                // Assert
                error.Should().BeNull();
                result.Should().NotBeNull();
                result!.Id.Should().Be(expenseId);
                result.Amount.Should().Be(dto.Amount);
                result.Description.Should().Be(dto.Description);
                result.UnidadeId.Should().Be(_unidadeId);
                result.CategoryId.Should().Be(_categoriaId);
                
                _repositoryMock.Verify(r => r.AddAsync(It.Is<Expense>(e => 
                    e.UnidadeId == _unidadeId &&
                    e.CategoryId == _categoriaId &&
                    e.Amount == dto.Amount &&
                    e.Description == dto.Description &&
                    e.TenantId == _tenantId)), 
                    Times.Once);
                    
                _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
            }

            [Fact]
            public async Task QuandoDescriptionVazia_DeveRetornarErro()
            {
                // Arrange
                var dto = CreateValidExpenseDto();
                dto.Description = string.Empty;
                
                var unidade = CreateUnidade();
                var categoria = CreateCategoria();
                
                _unidadeRepoMock
                    .Setup(r => r.GetByIdAsync(_unidadeId, _tenantId))
                    .ReturnsAsync(unidade);
                
                _repositoryMock
                    .Setup(r => r.GetCategoryByIdAsync(_categoriaId, _tenantId))
                    .ReturnsAsync(categoria);

                // Act
                var (result, error) = await _service.CreateExpenseAsync(dto, _tenantId);

                // Assert
                result.Should().BeNull();
                error.Should().Be("Description é obrigatória");
                
                _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Expense>()), Times.Never);
            }

            [Fact]
            public async Task QuandoAmountZero_DeveRetornarErro()
            {
                // Arrange
                var dto = CreateValidExpenseDto();
                dto.Amount = 0;
                
                var unidade = CreateUnidade();
                var categoria = CreateCategoria();
                
                _unidadeRepoMock
                    .Setup(r => r.GetByIdAsync(_unidadeId, _tenantId))
                    .ReturnsAsync(unidade);
                
                _repositoryMock
                    .Setup(r => r.GetCategoryByIdAsync(_categoriaId, _tenantId))
                    .ReturnsAsync(categoria);

                // Act
                var (result, error) = await _service.CreateExpenseAsync(dto, _tenantId);

                // Assert
                result.Should().BeNull();
                error.Should().Be("Amount deve ser maior que zero");
                
                _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Expense>()), Times.Never);
            }
        }

        public class GetExpensesByUnidadeAsyncTests : ExpenseServiceTests
        {
            [Fact]
            public async Task QuandoExistemExpenses_DeveRetornarLista()
            {
                // Arrange
                var unidade = CreateUnidade();
                var categoria = CreateCategoria();
                var expenses = new List<Expense>
                {
                    CreateExpense(Guid.NewGuid(), unidade, categoria),
                    CreateExpense(Guid.NewGuid(), unidade, categoria)
                };

                _repositoryMock
                    .Setup(r => r.GetAllByUnidadeAsync(_unidadeId, _tenantId))
                    .ReturnsAsync(expenses);

                // Act
                var result = await _service.GetExpensesByUnidadeAsync(_unidadeId, _tenantId);

                // Assert
                result.Should().NotBeNull();
                result.Should().HaveCount(2);
                
                _repositoryMock.Verify(r => r.GetAllByUnidadeAsync(_unidadeId, _tenantId), Times.Once);
            }

            [Fact]
            public async Task QuandoNaoExistemExpenses_DeveRetornarListaVazia()
            {
                // Arrange
                _repositoryMock
                    .Setup(r => r.GetAllByUnidadeAsync(_unidadeId, _tenantId))
                    .ReturnsAsync(new List<Expense>());

                // Act
                var result = await _service.GetExpensesByUnidadeAsync(_unidadeId, _tenantId);

                // Assert
                result.Should().NotBeNull();
                result.Should().BeEmpty();
            }
        }

        public class GetExpenseByIdAsyncTests : ExpenseServiceTests
        {
            [Fact]
            public async Task QuandoExpenseExiste_DeveRetornarExpense()
            {
                // Arrange
                var expenseId = Guid.NewGuid();
                var expense = CreateExpense(expenseId, CreateUnidade(), CreateCategoria());
                
                _repositoryMock
                    .Setup(r => r.GetByIdAsync(expenseId, _tenantId))
                    .ReturnsAsync(expense);

                // Act
                var result = await _service.GetExpenseByIdAsync(expenseId, _tenantId);

                // Assert
                result.Should().NotBeNull();
                result!.Id.Should().Be(expenseId);
                result.Amount.Should().Be(expense.Amount);
                result.UnidadeId.Should().Be(_unidadeId);
                
                _repositoryMock.Verify(r => r.GetByIdAsync(expenseId, _tenantId), Times.Once);
            }

            [Fact]
            public async Task QuandoExpenseNaoExiste_DeveRetornarNull()
            {
                // Arrange
                var expenseId = Guid.NewGuid();
                
                _repositoryMock
                    .Setup(r => r.GetByIdAsync(expenseId, _tenantId))
                    .ReturnsAsync((Expense?)null);

                // Act
                var result = await _service.GetExpenseByIdAsync(expenseId, _tenantId);

                // Assert
                result.Should().BeNull();
                
                _repositoryMock.Verify(r => r.GetByIdAsync(expenseId, _tenantId), Times.Once);
            }
        }

        public class DeleteExpenseAsyncTests : ExpenseServiceTests
        {
            [Fact]
            public async Task QuandoExpenseExiste_DeveDeletarComSucesso()
            {
                // Arrange
                var expenseId = Guid.NewGuid();
                var expense = CreateExpense(expenseId, CreateUnidade(), CreateCategoria());
                
                _repositoryMock
                    .Setup(r => r.GetByIdAsync(expenseId, _tenantId))
                    .ReturnsAsync(expense);

                // Act
                var (success, error) = await _service.DeleteExpenseAsync(expenseId, _tenantId);

                // Assert
                success.Should().BeTrue();
                error.Should().BeNull();
                
                _repositoryMock.Verify(r => r.Remove(expense), Times.Once);
                _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
            }

            [Fact]
            public async Task QuandoExpenseNaoExiste_DeveRetornarErro()
            {
                // Arrange
                var expenseId = Guid.NewGuid();
                
                _repositoryMock
                    .Setup(r => r.GetByIdAsync(expenseId, _tenantId))
                    .ReturnsAsync((Expense?)null);

                // Act
                var (success, error) = await _service.DeleteExpenseAsync(expenseId, _tenantId);

                // Assert
                success.Should().BeFalse();
                error.Should().Be(ErrorMessages.DespesaNotFound);
                
                _repositoryMock.Verify(r => r.Remove(It.IsAny<Expense>()), Times.Never);
            }
        }

        public class CategoryTests : ExpenseServiceTests
        {
            [Fact]
            public async Task CreateCategoryAsync_QuandoDadosValidos_DeveCriarCategoria()
            {
                // Arrange
                var dto = new ExpenseCategoryCreateDto 
                { 
                    Name = "Nova Categoria", 
                    Description = "Descrição da categoria" 
                };

                var categoriasExistentes = new List<ExpenseCategory>();
                _repositoryMock
                    .Setup(r => r.GetAllCategoriesAsync(_tenantId))
                    .ReturnsAsync(categoriasExistentes);

                _repositoryMock
                    .Setup(r => r.AddCategoryAsync(It.IsAny<ExpenseCategory>()))
                    .Returns(Task.CompletedTask);

                // Act
                var (result, error) = await _service.CreateCategoryAsync(dto, _tenantId);

                // Assert
                error.Should().BeNull();
                result.Should().NotBeNull();
                result!.Name.Should().Be(dto.Name);
                result.Description.Should().Be(dto.Description);
                
                _repositoryMock.Verify(r => r.AddCategoryAsync(It.Is<ExpenseCategory>(c => 
                    c.Name == dto.Name &&
                    c.Description == dto.Description &&
                    c.TenantId == _tenantId)), 
                    Times.Once);
                    
                _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
            }

            [Fact]
            public async Task CreateCategoryAsync_QuandoCategoriaDuplicada_DeveRetornarErro()
            {
                // Arrange
                var dto = new ExpenseCategoryCreateDto 
                { 
                    Name = "Categoria Existente", 
                    Description = "Descrição" 
                };

                var categoriasExistentes = new List<ExpenseCategory>
                {
                    new ExpenseCategory { Id = Guid.NewGuid(), Name = "Categoria Existente", TenantId = _tenantId }
                };

                _repositoryMock
                    .Setup(r => r.GetAllCategoriesAsync(_tenantId))
                    .ReturnsAsync(categoriasExistentes);

                // Act
                var (result, error) = await _service.CreateCategoryAsync(dto, _tenantId);

                // Assert
                result.Should().BeNull();
                error.Should().Be("Já existe uma categoria com este nome");
                
                _repositoryMock.Verify(r => r.AddCategoryAsync(It.IsAny<ExpenseCategory>()), Times.Never);
            }

            [Fact]
            public async Task GetCategoriesAsync_DeveRetornarListaDeCategorias()
            {
                // Arrange
                var categorias = new List<ExpenseCategory>
                {
                    new ExpenseCategory { Id = Guid.NewGuid(), Name = "Categoria 1", TenantId = _tenantId },
                    new ExpenseCategory { Id = Guid.NewGuid(), Name = "Categoria 2", TenantId = _tenantId }
                };

                _repositoryMock
                    .Setup(r => r.GetAllCategoriesAsync(_tenantId))
                    .ReturnsAsync(categorias);

                // Act
                var result = await _service.GetCategoriesAsync(_tenantId);

                // Assert
                result.Should().NotBeNull();
                result.Should().HaveCount(2);
                
                _repositoryMock.Verify(r => r.GetAllCategoriesAsync(_tenantId), Times.Once);
            }
        }

        // Helper methods
        private ExpenseCreateDto CreateValidExpenseDto()
        {
            return new ExpenseCreateDto
            {
                UnidadeId = _unidadeId,
                CategoryId = _categoriaId,
                Amount = 100.50m,
                Description = "Despesa de teste",
                ExpenseDate = DateTime.UtcNow.Date
            };
        }

        private Unidade CreateUnidade()
        {
            return new Unidade { Id = _unidadeId, Nome = "Unidade Teste" };
        }

        private ExpenseCategory CreateCategoria()
        {
            return new ExpenseCategory { Id = _categoriaId, Name = "Categoria Teste" };
        }

        private Expense CreateExpense(
            Guid id, 
            Unidade unidade, 
            ExpenseCategory categoria, 
            ExpenseCreateDto? dto = null)
        {
            dto ??= CreateValidExpenseDto();
            
            return new Expense
            {
                Id = id,
                UnidadeId = unidade.Id,
                CategoryId = categoria.Id,
                Amount = dto.Amount,
                Description = dto.Description,
                ExpenseDate = dto.ExpenseDate,
                TenantId = _tenantId,
                Unidade = unidade,
                Category = categoria
            };
        }
    }
}