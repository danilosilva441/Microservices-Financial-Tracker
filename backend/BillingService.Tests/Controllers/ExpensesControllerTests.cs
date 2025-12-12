using BillingService.Controllers;
using BillingService.DTOs;
using BillingService.Models;
using BillingService.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SharedKernel;
using System.Security.Claims;
using Xunit;

namespace BillingService.Tests.Controllers
{
    public class ExpensesControllerTests
    {
        private readonly Mock<IExpenseService> _expenseServiceMock;
        private readonly Mock<ILogger<ExpensesController>> _loggerMock;
        private readonly ExpensesController _controller;
        private readonly Guid _tenantId;
        private readonly Guid _userId;

        public ExpensesControllerTests()
        {
            _expenseServiceMock = new Mock<IExpenseService>();
            _loggerMock = new Mock<ILogger<ExpensesController>>();
            
            _controller = new ExpensesController(
                _expenseServiceMock.Object, 
                _loggerMock.Object);
            
            // Dados de teste
            _userId = Guid.NewGuid();
            _tenantId = Guid.NewGuid();
            
            SetupControllerContext();
        }

        private void SetupControllerContext(bool withTenantId = true, bool withAdminRole = true)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, _userId.ToString())
            };

            if (withTenantId)
            {
                claims.Add(new Claim("tenantId", _tenantId.ToString()));
            }

            if (withAdminRole)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            }

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);
            
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task CreateCategory_Sucesso_DeveRetornarCreated()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var dto = new ExpenseCategoryCreateDto 
            { 
                Name = "Categoria Teste",
                Description = "Descrição da categoria"
            };
            
            var expectedCategory = new ExpenseCategory 
            { 
                Id = categoryId,
                Name = dto.Name,
                TenantId = _tenantId
            };
            
            _expenseServiceMock
                .Setup(s => s.CreateCategoryAsync(dto, _tenantId))
                .ReturnsAsync((expectedCategory, null));

            // Act
            var result = await _controller.CreateCategory(dto);

            // Assert
            var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.ActionName.Should().Be(nameof(ExpensesController.GetCategories));
            createdResult.Value.Should().BeEquivalentTo(expectedCategory);
            
            _expenseServiceMock.Verify(
                s => s.CreateCategoryAsync(dto, _tenantId), 
                Times.Once);
        }

        [Fact]
        public async Task CreateCategory_ErroNoServico_DeveRetornarBadRequest()
        {
            // Arrange
            var dto = new ExpenseCategoryCreateDto { Name = "Categoria Teste" };
            var errorMessage = "Categoria já existe";
            
            _expenseServiceMock
                .Setup(s => s.CreateCategoryAsync(dto, _tenantId))
                .ReturnsAsync((null, errorMessage));

            // Act
            var result = await _controller.CreateCategory(dto);

            // Assert
            var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequest.Value.Should().Be(errorMessage);
        }

        [Fact]
        public async Task CreateExpense_UnidadeInvalida_DeveRetornarNotFound()
        {
            // Arrange
            var dto = new ExpenseCreateDto
            {
                Amount = 100.50m,
                Description = "Despesa de teste",
                // Ajuste: Use a propriedade correta conforme seu DTO real
                // Se for UnidadeId, descomente a linha abaixo
                // UnidadeId = Guid.NewGuid()
            };
            
            _expenseServiceMock
                .Setup(s => s.CreateExpenseAsync(dto, _tenantId))
                .ReturnsAsync((null, ErrorMessages.UnidadeNotFound));

            // Act
            var result = await _controller.CreateExpense(dto);

            // Assert
            var notFound = result.Should().BeOfType<NotFoundObjectResult>().Subject;
            notFound.Value.Should().Be(ErrorMessages.UnidadeNotFound);
        }

        [Fact]
        public async Task CreateExpense_Sucesso_DeveRetornarCreated()
        {
            // Arrange
            var expenseId = Guid.NewGuid();
            var dto = new ExpenseCreateDto
            {
                Amount = 100.50m,
                Description = "Despesa de teste",
                CategoryId = Guid.NewGuid()
            };
            
            var expectedExpense = new Expense
            {
                Id = expenseId,
                Amount = dto.Amount,
                Description = dto.Description,
                TenantId = _tenantId
            };
            
            _expenseServiceMock
                .Setup(s => s.CreateExpenseAsync(dto, _tenantId))
                .ReturnsAsync((expectedExpense, null));

            // Act
            var result = await _controller.CreateExpense(dto);

            // Assert
            var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.ActionName.Should().Be(nameof(ExpensesController.GetExpensesByUnidade));
            createdResult.Value.Should().BeEquivalentTo(expectedExpense);
        }

        [Fact]
        public async Task CreateExpense_UsuarioNaoAutenticado_DeveRetornarUnauthorized()
        {
            // Arrange
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
            };
            
            var dto = new ExpenseCreateDto();

            // Act
            var result = await _controller.CreateExpense(dto);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task GetExpensesByUnidade_Sucesso_DeveRetornarOk()
        {
            // Arrange
            var unidadeId = Guid.NewGuid();
            var expectedExpenses = new List<Expense>
            {
                new Expense { Id = Guid.NewGuid(), Amount = 100m, TenantId = _tenantId },
                new Expense { Id = Guid.NewGuid(), Amount = 200m, TenantId = _tenantId }
            };
            
            _expenseServiceMock
                .Setup(s => s.GetExpensesByUnidadeAsync(unidadeId, _tenantId))
                .ReturnsAsync(expectedExpenses);

            // Act
            var result = await _controller.GetExpensesByUnidade(unidadeId);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(expectedExpenses);
        }

        [Fact]
        public async Task GetExpensesByUnidade_UnidadeVazia_DeveRetornarBadRequest()
        {
            // Arrange
            var unidadeId = Guid.Empty;

            // Act
            var result = await _controller.GetExpensesByUnidade(unidadeId);

            // Assert
            var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequest.Value.Should().Be("Invalid unidade ID.");
        }

        [Fact]
        public async Task GetCategories_Sucesso_DeveRetornarOk()
        {
            // Arrange
            var expectedCategories = new List<ExpenseCategory>
            {
                new ExpenseCategory { Id = Guid.NewGuid(), Name = "Categoria 1", TenantId = _tenantId },
                new ExpenseCategory { Id = Guid.NewGuid(), Name = "Categoria 2", TenantId = _tenantId }
            };
            
            _expenseServiceMock
                .Setup(s => s.GetCategoriesAsync(_tenantId))
                .ReturnsAsync(expectedCategories);

            // Act
            var result = await _controller.GetCategories();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(expectedCategories);
        }

        [Fact]
        public async Task DeleteExpense_Sucesso_DeveRetornarNoContent()
        {
            // Arrange
            var expenseId = Guid.NewGuid();
            
            _expenseServiceMock
                .Setup(s => s.DeleteExpenseAsync(expenseId, _tenantId))
                .ReturnsAsync((true, null));

            // Act
            var result = await _controller.DeleteExpense(expenseId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeleteExpense_NaoEncontrado_DeveRetornarNotFound()
        {
            // Arrange
            var expenseId = Guid.NewGuid();
            var errorMessage = "Expense not found";
            
            _expenseServiceMock
                .Setup(s => s.DeleteExpenseAsync(expenseId, _tenantId))
                .ReturnsAsync((false, errorMessage));

            // Act
            var result = await _controller.DeleteExpense(expenseId);

            // Assert
            var notFound = result.Should().BeOfType<NotFoundObjectResult>().Subject;
            notFound.Value.Should().Be(errorMessage);
        }

        [Fact]
        public async Task DeleteExpense_IdVazio_DeveRetornarBadRequest()
        {
            // Arrange
            var expenseId = Guid.Empty;

            // Act
            var result = await _controller.DeleteExpense(expenseId);

            // Assert
            var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequest.Value.Should().Be("Invalid expense ID.");
        }

        [Fact]
        public async Task GetTenantId_ClaimAusente_DeveRetornarUnauthorized()
        {
            // Arrange - Configurar contexto sem tenantId
            SetupControllerContext(withTenantId: false);
            
            var dto = new ExpenseCreateDto();

            // Act
            var result = await _controller.CreateExpense(dto);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
            var unauthorizedResult = result as UnauthorizedObjectResult;
            unauthorizedResult!.Value!.ToString()!.Should().Contain("Tenant ID not found");
        }

        [Fact]
        public async Task GetTenantId_ClaimInvalida_DeveRetornarUnauthorized()
        {
            // Arrange - Configurar contexto com tenantId inválido
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, _userId.ToString()),
                new Claim("tenantId", "invalido") // GUID inválido
            };
            
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);
            
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
            
            var dto = new ExpenseCreateDto();

            // Act
            var result = await _controller.CreateExpense(dto);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
            var unauthorizedResult = result as UnauthorizedObjectResult;
            unauthorizedResult!.Value!.ToString()!.Should().Contain("Invalid Tenant ID format");
        }

        [Fact]
        public async Task CreateExpense_TenantIdInvalido_DeveRetornarUnauthorized()
        {
            // Arrange - Configurar contexto sem tenantId
            SetupControllerContext(withTenantId: false);
            
            var dto = new ExpenseCreateDto();

            // Act
            var result = await _controller.CreateExpense(dto);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task UploadExpenses_ArquivoNulo_DeveRetornarBadRequest()
        {
            // Arrange
            IFormFile? file = null;

            // Act
            var result = await _controller.UploadExpenses(file!);

            // Assert
            var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequest.Value.Should().Be("No file was uploaded.");
        }

        [Fact]
        public async Task UploadExpenses_ExtensaoInvalida_DeveRetornarBadRequest()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("arquivo.txt");
            fileMock.Setup(f => f.Length).Returns(1000);
            fileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());

            // Act
            var result = await _controller.UploadExpenses(fileMock.Object);

            // Assert
            var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequest.Value!.ToString()!.Should().Contain("Invalid file format");
        }

        // Testes para upload de arquivo
        [Fact]
        public async Task UploadExpenses_Sucesso_DeveRetornarOk()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("arquivo.xlsx");
            fileMock.Setup(f => f.Length).Returns(1000);
            fileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());
            
            // Corrigido: Especificar os tipos explicitamente
            (bool success, string? errorMessage, int processedRows, List<int> skippedRows) expectedResult = 
                (true, null, 10, new List<int> { 2, 5 });
            
            _expenseServiceMock
                .Setup(s => s.ImportExpensesAsync(It.IsAny<Stream>(), ".xlsx", _tenantId))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.UploadExpenses(fileMock.Object);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().NotBeNull();
            
            _expenseServiceMock.Verify(
                s => s.ImportExpensesAsync(It.IsAny<Stream>(), ".xlsx", _tenantId),
                Times.Once);
        }

        [Fact]
        public async Task UploadExpenses_ErroImportacao_DeveRetornarBadRequest()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("arquivo.xlsx");
            fileMock.Setup(f => f.Length).Returns(1000);
            fileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());
            
            // Corrigido: Especificar os tipos explicitamente
            (bool success, string? errorMessage, int processedRows, List<int> skippedRows) expectedResult = 
                (false, "Erro na importação", 0, new List<int>());
            
            _expenseServiceMock
                .Setup(s => s.ImportExpensesAsync(It.IsAny<Stream>(), ".xlsx", _tenantId))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.UploadExpenses(fileMock.Object);

            // Assert
            var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequest.Value.Should().NotBeNull();
        }

        [Fact]
        public async Task UploadExpenses_ArquivoMuitoGrande_DeveRetornarRequestTooLarge()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("arquivo.xlsx");
            fileMock.Setup(f => f.Length).Returns(11 * 1024 * 1024); // 11MB > 10MB limite
            fileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());

            // Act
            var result = await _controller.UploadExpenses(fileMock.Object);

            // Assert
            var statusCodeResult = result.Should().BeOfType<ObjectResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(413); // Request Entity Too Large
            statusCodeResult.Value.Should().Be("File size exceeds the maximum limit of 10MB.");
        }
    }
}