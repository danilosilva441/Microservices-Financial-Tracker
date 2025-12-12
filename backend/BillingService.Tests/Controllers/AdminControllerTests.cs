using BillingService.Controllers;
using BillingService.Data;
using BillingService.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Reflection;
using System.Security.Claims;

namespace BillingService.Tests.Controllers;

public class AdminControllerTests
{
    private readonly DbContextOptions<BillingDbContext> _dbOptions;
    private readonly Mock<ILogger<AdminController>> _loggerMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly Guid _tenantId;

    public AdminControllerTests()
    {
        // 1. Configura Banco em Memória
        _dbOptions = new DbContextOptionsBuilder<BillingDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _loggerMock = new Mock<ILogger<AdminController>>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _tenantId = Guid.NewGuid();

        // 2. Configura o Mock do HttpContext para o DbContext ler o TenantId corretamente
        var context = new DefaultHttpContext();
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new Claim("tenantId", _tenantId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        
        context.User = new ClaimsPrincipal(identity);

        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);
    }

    // Método corrigido para criar o DbContext com ambos os parâmetros
    private BillingDbContext GetDbContext()
    {
        return new BillingDbContext(_dbOptions, _httpContextAccessorMock.Object);
    }

    private AdminController CreateController()
    {
        // Usa o método GetDbContext() que cria o contexto corretamente
        var context = GetDbContext();
        var controller = new AdminController(context, _loggerMock.Object);
        
        // Garante que o Controller tenha o mesmo HttpContext que o Banco
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = _httpContextAccessorMock.Object.HttpContext!
        };

        return controller;
    }

    #region VincularUsuarioUnidade

    [Fact]
    public async Task VincularUsuarioUnidade_DeveRetornarOk_QuandoUsuarioNovoEUnidadeExiste()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var unidadeId = Guid.NewGuid();

        using var context = GetDbContext();
        var unidadeSeed = new Unidade { Id = unidadeId, Nome = "Loja Matriz", TenantId = _tenantId };
        
        await context.Unidades.AddAsync(unidadeSeed);
        await context.SaveChangesAsync();

        var controller = CreateController();
        var dto = new AdminController.VincularUsuarioDto(userId, unidadeId);

        // Act
        var result = await controller.VincularUsuarioUnidade(dto);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        
        var vinculoSalvo = await context.UsuarioOperacoes
            .FirstOrDefaultAsync(u => u.UserId == userId && u.UnidadeId == unidadeId);
        
        vinculoSalvo.Should().NotBeNull();
        vinculoSalvo!.TenantId.Should().Be(_tenantId);
    }

    [Fact]
    public async Task VincularUsuarioUnidade_DeveRetornarNotFound_QuandoUnidadeNaoExiste()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var controller = CreateController();
        var dto = new AdminController.VincularUsuarioDto(userId, Guid.NewGuid());

        // Act
        var result = await controller.VincularUsuarioUnidade(dto);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>()
            .Which.Value.Should().Be("Unidade não encontrada ou não pertence a este tenant.");
    }

    [Fact]
    public async Task VincularUsuarioUnidade_DeveRetornarConflict_QuandoVinculoJaExiste()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var unidadeId = Guid.NewGuid();

        using var context = GetDbContext();
        var unidade = new Unidade { Id = unidadeId, Nome = "Loja Teste", TenantId = _tenantId };
        var vinculoExistente = new UsuarioOperacao 
        { 
            Id = Guid.NewGuid(), 
            UserId = userId, 
            UnidadeId = unidadeId, 
            TenantId = _tenantId 
        };

        await context.Unidades.AddAsync(unidade);
        await context.UsuarioOperacoes.AddAsync(vinculoExistente);
        await context.SaveChangesAsync();

        var controller = CreateController();
        var dto = new AdminController.VincularUsuarioDto(userId, unidadeId);

        // Act
        var result = await controller.VincularUsuarioUnidade(dto);

        // Assert
        result.Should().BeOfType<ConflictObjectResult>()
            .Which.Value.Should().Be("Este usuário já está vinculado a esta unidade.");
    }

    [Fact]
    public async Task VincularUsuarioUnidade_DeveRetornarUnauthorized_QuandoTenantIdInvalido()
    {
        // Arrange
        using var context = GetDbContext();
        var controller = new AdminController(context, _loggerMock.Object);
        
        // Simula um contexto sem TenantId (usuário não autenticado)
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
            // SEM tenantId no token
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext 
            { 
                User = new ClaimsPrincipal(identity) 
            }
        };

        var dto = new AdminController.VincularUsuarioDto(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var result = await controller.VincularUsuarioUnidade(dto);

        // Assert
        result.Should().BeOfType<UnauthorizedObjectResult>()
            .Which.Value.Should().Be("Tenant ID não autorizado.");
    }

    [Fact]
    public async Task VincularUsuarioUnidade_DeveRetornarInternalServerError_QuandoDbUpdateException()
    {
        // Arrange - Abordagem diferente: usar um contexto real que lança exceção
        var userId = Guid.NewGuid();
        var unidadeId = Guid.NewGuid();

        // Cria um contexto real
        var dbOptions = new DbContextOptionsBuilder<BillingDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        // Criamos um DbContext personalizado que lança exceção
        var exceptionContext = new ExceptionThrowingDbContext(dbOptions, _httpContextAccessorMock.Object);
        
        // Adiciona a unidade
        var unidadeSeed = new Unidade { Id = unidadeId, Nome = "Loja Teste", TenantId = _tenantId };
        await exceptionContext.Unidades.AddAsync(unidadeSeed);
        await exceptionContext.SaveChangesAsync();
        
        // Configura para lançar exceção no próximo SaveChangesAsync
        exceptionContext.ThrowOnSaveChanges = true;

        var controller = new AdminController(exceptionContext, _loggerMock.Object);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = _httpContextAccessorMock.Object.HttpContext!
        };

        var dto = new AdminController.VincularUsuarioDto(userId, unidadeId);

        // Act
        var result = await controller.VincularUsuarioUnidade(dto);

        // Assert
        result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }

    [Fact]
    public async Task VincularUsuarioUnidade_DeveRetornarNotFound_QuandoUnidadeDeOutroTenant()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var unidadeId = Guid.NewGuid();
        var outroTenantId = Guid.NewGuid(); // Tenant diferente

        using var context = GetDbContext();
        // Cria unidade com tenant diferente
        var unidadeSeed = new Unidade { Id = unidadeId, Nome = "Loja Outro Tenant", TenantId = outroTenantId };
        
        await context.Unidades.AddAsync(unidadeSeed);
        await context.SaveChangesAsync();

        var controller = CreateController();
        var dto = new AdminController.VincularUsuarioDto(userId, unidadeId);

        // Act
        var result = await controller.VincularUsuarioUnidade(dto);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>()
            .Which.Value.Should().Be("Unidade não encontrada ou não pertence a este tenant.");
    }

    #endregion

    #region GetVinculosUsuario

    [Fact]
    public async Task GetVinculosUsuario_DeveRetornarLista_QuandoExistemVinculos()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var unidadeId = Guid.NewGuid();

        using var context = GetDbContext();
        var unidade = new Unidade { Id = unidadeId, Nome = "Loja Teste", TenantId = _tenantId };
        var vinculo = new UsuarioOperacao 
        { 
            Id = Guid.NewGuid(), 
            UserId = userId, 
            UnidadeId = unidadeId, 
            TenantId = _tenantId 
        };

        await context.Unidades.AddAsync(unidade);
        await context.UsuarioOperacoes.AddAsync(vinculo);
        await context.SaveChangesAsync();

        var controller = CreateController();

        // Act
        var result = await controller.GetVinculosUsuario(userId);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var lista = okResult.Value as IEnumerable<object>;
        
        lista.Should().NotBeNull();
        lista.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetVinculosUsuario_DeveRetornarNotFound_QuandoNaoExistemVinculos()
    {
        // Arrange
        var controller = CreateController();

        // Act
        var result = await controller.GetVinculosUsuario(Guid.NewGuid());

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>()
            .Which.Value.Should().Be("Nenhum vínculo encontrado para este usuário.");
    }

    [Fact]
    public async Task GetVinculosUsuario_DeveRetornarBadRequest_QuandoUserIdVazio()
    {
        // Arrange
        var controller = CreateController();

        // Act
        var result = await controller.GetVinculosUsuario(Guid.Empty);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().Be("UserId é obrigatório.");
    }

    [Fact]
    public async Task GetVinculosUsuario_DeveRetornarApenasVinculosDoTenant()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var unidadeId1 = Guid.NewGuid();
        var unidadeId2 = Guid.NewGuid();
        var outroTenantId = Guid.NewGuid();

        using var context = GetDbContext();
        
        // Unidade do mesmo tenant
        var unidade1 = new Unidade { Id = unidadeId1, Nome = "Loja Tenant Atual", TenantId = _tenantId };
        var vinculo1 = new UsuarioOperacao 
        { 
            Id = Guid.NewGuid(), 
            UserId = userId, 
            UnidadeId = unidadeId1, 
            TenantId = _tenantId 
        };

        // Unidade de outro tenant (não deve aparecer)
        var unidade2 = new Unidade { Id = unidadeId2, Nome = "Loja Outro Tenant", TenantId = outroTenantId };
        var vinculo2 = new UsuarioOperacao 
        { 
            Id = Guid.NewGuid(), 
            UserId = userId, 
            UnidadeId = unidadeId2, 
            TenantId = outroTenantId 
        };

        await context.Unidades.AddRangeAsync(unidade1, unidade2);
        await context.UsuarioOperacoes.AddRangeAsync(vinculo1, vinculo2);
        await context.SaveChangesAsync();

        var controller = CreateController();

        // Act
        var result = await controller.GetVinculosUsuario(userId);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var lista = okResult.Value as IEnumerable<dynamic>;
        
        lista.Should().NotBeNull();
        lista.Should().HaveCount(1); // Apenas 1 vínculo do tenant atual
    }

    #endregion

    #region Testes de Integração

    [Fact]
    public async Task VincularEDepoisListar_DeveFuncionarCorretamente()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var unidadeId = Guid.NewGuid();

        using var context = GetDbContext();
        var unidade = new Unidade { Id = unidadeId, Nome = "Loja Integração", TenantId = _tenantId };
        
        await context.Unidades.AddAsync(unidade);
        await context.SaveChangesAsync();

        var controller = CreateController();
        var dto = new AdminController.VincularUsuarioDto(userId, unidadeId);

        // Act 1: Vincular
        var resultadoVinculo = await controller.VincularUsuarioUnidade(dto);

        // Assert 1
        resultadoVinculo.Should().BeOfType<OkObjectResult>();

        // Act 2: Listar vínculos
        var resultadoLista = await controller.GetVinculosUsuario(userId);

        // Assert 2
        var okResult = resultadoLista.Should().BeOfType<OkObjectResult>().Subject;
        var lista = okResult.Value as IEnumerable<dynamic>;
        lista.Should().NotBeNull();
        lista.Should().HaveCount(1);
    }

    [Fact]
    public void GetTenantIdFromToken_DeveLancarExcecao_QuandoTenantIdInvalido()
    {
        // Arrange - Teste direto do método GetTenantIdFromToken usando reflexão
        using var context = GetDbContext();
        var controller = new AdminController(context, _loggerMock.Object);
        
        // Usuário sem tenantId no token
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext 
            { 
                User = new ClaimsPrincipal(identity) 
            }
        };

        // Act - Invoca o método privado GetTenantIdFromToken usando reflexão
        var method = typeof(AdminController).GetMethod("GetTenantIdFromToken", 
            BindingFlags.NonPublic | BindingFlags.Instance);
        
        // Assert - Verifica se lança a exceção (forma simplificada)
        Action act = () => method!.Invoke(controller, null);
        
        act.Should().Throw<TargetInvocationException>()
           .WithInnerException<UnauthorizedAccessException>()
           .WithMessage("Tenant ID não autorizado.");
    }

    #endregion

    #region Testes Adicionais para Validação

    [Fact]
    public async Task VincularUsuarioUnidade_DeveValidarEntrada_QuandoDtoNulo()
    {
        // Arrange
        var controller = CreateController();

        // Act
        var result = await controller.VincularUsuarioUnidade(null!);

        // Assert - O controller deve retornar BadRequest para DTO nulo (se você adicionou a validação)
        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().Be("DTO é obrigatório.");
    }

    [Fact]
    public async Task VincularUsuarioUnidade_DeveValidarEntrada_QuandoIdsInvalidos()
    {
        // Arrange
        var controller = CreateController();
        
        // Teste 1: UserId vazio
        var dto1 = new AdminController.VincularUsuarioDto(Guid.Empty, Guid.NewGuid());
        var result1 = await controller.VincularUsuarioUnidade(dto1);
        result1.Should().BeOfType<BadRequestObjectResult>()
               .Which.Value.Should().Be("UserId e UnidadeId são obrigatórios.");

        // Teste 2: UnidadeId vazio
        var dto2 = new AdminController.VincularUsuarioDto(Guid.NewGuid(), Guid.Empty);
        var result2 = await controller.VincularUsuarioUnidade(dto2);
        result2.Should().BeOfType<BadRequestObjectResult>()
               .Which.Value.Should().Be("UserId e UnidadeId são obrigatórios.");

        // Teste 3: Ambos vazios
        var dto3 = new AdminController.VincularUsuarioDto(Guid.Empty, Guid.Empty);
        var result3 = await controller.VincularUsuarioUnidade(dto3);
        result3.Should().BeOfType<BadRequestObjectResult>()
               .Which.Value.Should().Be("UserId e UnidadeId são obrigatórios.");
    }

    [Fact]
    public async Task VincularUsuarioUnidade_DeveLogarInformacoes_QuandoSucesso()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var unidadeId = Guid.NewGuid();

        using var context = GetDbContext();
        var unidadeSeed = new Unidade { Id = unidadeId, Nome = "Loja Teste", TenantId = _tenantId };
        
        await context.Unidades.AddAsync(unidadeSeed);
        await context.SaveChangesAsync();

        var loggerMock = new Mock<ILogger<AdminController>>();
        var controller = new AdminController(context, loggerMock.Object);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = _httpContextAccessorMock.Object.HttpContext!
        };

        var dto = new AdminController.VincularUsuarioDto(userId, unidadeId);

        // Act
        var result = await controller.VincularUsuarioUnidade(dto);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        
        // Verifica se o log foi chamado
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Vínculo criado")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)!),
            Times.Once);
    }

    #endregion

    #region Testes de Validação de DTO

    [Fact]
    public async Task VincularUsuarioUnidade_DeveRetornarBadRequest_QuandoUserIdVazio()
    {
        // Arrange
        var controller = CreateController();
        var dto = new AdminController.VincularUsuarioDto(Guid.Empty, Guid.NewGuid());

        // Act
        var result = await controller.VincularUsuarioUnidade(dto);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().Be("UserId e UnidadeId são obrigatórios.");
    }

    [Fact]
    public async Task VincularUsuarioUnidade_DeveRetornarBadRequest_QuandoUnidadeIdVazio()
    {
        // Arrange
        var controller = CreateController();
        var dto = new AdminController.VincularUsuarioDto(Guid.NewGuid(), Guid.Empty);

        // Act
        var result = await controller.VincularUsuarioUnidade(dto);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().Be("UserId e UnidadeId são obrigatórios.");
    }

    #endregion
}

// Classe auxiliar para simular DbContext que lança exceção
public class ExceptionThrowingDbContext : BillingDbContext
{
    public bool ThrowOnSaveChanges { get; set; }

    public ExceptionThrowingDbContext(DbContextOptions<BillingDbContext> options, IHttpContextAccessor httpContextAccessor)
        : base(options, httpContextAccessor)
    {
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (ThrowOnSaveChanges)
        {
            throw new DbUpdateException("Erro de banco de dados simulado");
        }
        
        return base.SaveChangesAsync(cancellationToken);
    }
}