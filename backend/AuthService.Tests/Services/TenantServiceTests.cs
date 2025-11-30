using AuthService.DTO;
using AuthService.Models;
using AuthService.Repositories;
using AuthService.Services;
using AuthService.Data;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;
using Microsoft.Extensions.Logging;

namespace AuthService.Tests.Services
{
    public class TenantServiceIntegrationTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<AuthDbContext> _options;
        private readonly AuthDbContext _context;
        private readonly TenantService _service;
        private readonly TenantRepository _tenantRepo;
        private readonly UserRepository _userRepo;
        private readonly RoleRepository _roleRepo;
        private readonly Mock<ILogger<TenantService>> _loggerMock;

        public TenantServiceIntegrationTests()
        {
            // Configurar Banco SQLite em Memória
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            _options = new DbContextOptionsBuilder<AuthDbContext>()
                .UseSqlite(_connection)
                .Options;

            var mockAccessor = new Mock<IHttpContextAccessor>();
            _context = new AuthDbContext(_options, mockAccessor.Object);
            
            // Criar o banco e semear dados
            InitializeDatabase();

            // Inicializar repositórios
            _tenantRepo = new TenantRepository(_context);
            _userRepo = new UserRepository(_context);
            _roleRepo = new RoleRepository(_context);
            
            // Mock do logger
            _loggerMock = new Mock<ILogger<TenantService>>();
            
            // Inicializar serviço com logger mock
            _service = new TenantService(_userRepo, _roleRepo, _tenantRepo, _context, _loggerMock.Object);
        }

        private void InitializeDatabase()
        {
            _context.Database.EnsureCreated();

            // Semear roles necessárias para os testes
            if (!_context.Roles.Any())
            {
                var roles = new[]
                {
                    new Role { Name = "Gerente", NormalizedName = "GERENTE" },
                    new Role { Name = "Admin", NormalizedName = "ADMIN" },
                    new Role { Name = "User", NormalizedName = "USER" }
                };
                _context.Roles.AddRange(roles);
                _context.SaveChanges();
            }
        }

        public void Dispose()
        {
            _context?.Dispose();
            _connection?.Dispose();
        }

        [Fact]
        public async Task ProvisionTenantAsync_ComDadosValidos_DeveCriarTenantEGerenteComSucesso()
        {
            // Arrange
            var request = new TenantProvisionDto
            {
                NomeDaEmpresa = "Empresa Teste Ltda",
                EmailDoGerente = "gerente@empresateste.com",
                SenhaDoGerente = "SenhaForte123!"
            };

            // Act
            var result = await _service.ProvisionTenantAsync(request);

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();

            // Verificar se os dados foram persistidos corretamente
            var tenantDb = await _context.Tenants
                .FirstOrDefaultAsync(t => t.NomeDaEmpresa == request.NomeDaEmpresa);
            tenantDb.Should().NotBeNull();
            tenantDb!.NomeDaEmpresa.Should().Be(request.NomeDaEmpresa);
            tenantDb.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

            var userDb = await _context.Users
                .Include(u => u.Roles)
                .Include(u => u.Tenant)
                .FirstOrDefaultAsync(u => u.Email == request.EmailDoGerente);
            
            userDb.Should().NotBeNull();
            userDb!.Email.Should().Be(request.EmailDoGerente);
            userDb.TenantId.Should().Be(tenantDb.Id);
            userDb.Tenant!.NomeDaEmpresa.Should().Be(request.NomeDaEmpresa);
            userDb.Roles.Should().ContainSingle(r => r.Name == "Gerente");
            userDb.PasswordHash.Should().NotBeNullOrEmpty();
            userDb.PasswordHash.Should().NotBe(request.SenhaDoGerente);
        }

        [Fact]
        public async Task ProvisionTenantAsync_ComEmailDuplicado_DeveFalharENaoCriarTenant()
        {
            // Arrange
            var existingEmail = "existente@teste.com";
            var existingUser = new User 
            { 
                Email = existingEmail, 
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("senha123"),
                TenantId = null 
            };
            _context.Users.Add(existingUser);
            await _context.SaveChangesAsync();

            var request = new TenantProvisionDto
            {
                NomeDaEmpresa = "Nova Empresa",
                EmailDoGerente = existingEmail,
                SenhaDoGerente = "SenhaForte123!"
            };

            // Act
            var result = await _service.ProvisionTenantAsync(request);

            // Assert
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Contain("já existe");

            // Verificar que nenhum tenant foi criado
            var tenantCount = await _context.Tenants
                .CountAsync(t => t.NomeDaEmpresa == request.NomeDaEmpresa);
            tenantCount.Should().Be(0);
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        public async Task ProvisionTenantAsync_ComNomeDaEmpresaInvalido_DeveRetornarErro(string nomeInvalido)
        {
            // Arrange
            var request = new TenantProvisionDto
            {
                NomeDaEmpresa = nomeInvalido,
                EmailDoGerente = "gerente@teste.com",
                SenhaDoGerente = "SenhaForte123!"
            };

            // Act
            var result = await _service.ProvisionTenantAsync(request);

            // Assert
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().NotBeNullOrEmpty();
        }

        [Theory]
        [InlineData("email-invalido")]
        [InlineData("sem-arroba.com")]
        [InlineData("@sem-prefixo.com")]
        public async Task ProvisionTenantAsync_ComEmailInvalido_DeveRetornarErro(string emailInvalido)
        {
            // Arrange
            var request = new TenantProvisionDto
            {
                NomeDaEmpresa = "Empresa Teste",
                EmailDoGerente = emailInvalido,
                SenhaDoGerente = "SenhaForte123!"
            };

            // Act
            var result = await _service.ProvisionTenantAsync(request);

            // Assert
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Contain("email");
        }

        [Theory]
        [InlineData("123")] // Muito curta
        [InlineData("senhasemmaiuscula")] // Sem maiúscula (todas minúsculas)
        [InlineData("SENHASEMminuscula")] // Sem minúscula (todas maiúsculas)  
        [InlineData("SenhaSemNumero")] // Sem número
        public async Task ProvisionTenantAsync_ComSenhaFraca_DeveRetornarErro(string senhaFraca)
        {
            // Arrange
            var request = new TenantProvisionDto
            {
                NomeDaEmpresa = "Empresa Teste",
                EmailDoGerente = "gerente@teste.com",
                SenhaDoGerente = senhaFraca
            };

            // Act
            var result = await _service.ProvisionTenantAsync(request);

            // Assert
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Contain("senha");
        }

        [Fact]
        public async Task ProvisionTenantAsync_ComRoleGerenteNaoExistente_DeveRetornarErro()
        {
            // Arrange - Limpar roles para simular role faltante
            _context.Roles.RemoveRange(_context.Roles);
            await _context.SaveChangesAsync();

            var request = new TenantProvisionDto
            {
                NomeDaEmpresa = "Empresa Teste",
                EmailDoGerente = "gerente@teste.com",
                SenhaDoGerente = "SenhaForte123!"
            };

            // Criar novo serviço com contexto atualizado
            var loggerMock = new Mock<ILogger<TenantService>>();
            var service = new TenantService(_userRepo, _roleRepo, _tenantRepo, _context, loggerMock.Object);

            // Act
            var result = await service.ProvisionTenantAsync(request);

            // Assert
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetTenantUsersAsync_ComTenantExistente_DeveRetornarListaDeUsuarios()
        {
            // Arrange - Criar um tenant com usuários
            var tenant = new Tenant { NomeDaEmpresa = "Empresa com Usuários" };
            _context.Tenants.Add(tenant);
            await _context.SaveChangesAsync();

            var users = new[]
            {
                new User { Email = "user1@empresa.com", PasswordHash = "hash1", TenantId = tenant.Id },
                new User { Email = "user2@empresa.com", PasswordHash = "hash2", TenantId = tenant.Id },
                new User { Email = "user3@empresa.com", PasswordHash = "hash3", TenantId = tenant.Id }
            };
            _context.Users.AddRange(users);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetTenantUsersAsync(tenant.Id);

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            
            // Para listas, podemos verificar se é uma coleção enumerável
            if (result.Data is System.Collections.IEnumerable enumerable)
            {
                var count = 0;
                foreach (var item in enumerable) count++;
                count.Should().Be(3);
            }
        }

        [Fact]
        public async Task GetTenantUsersAsync_ComTenantInexistente_DeveRetornarErro()
        {
            // Arrange
            var tenantIdInexistente = Guid.NewGuid();

            // Act
            var result = await _service.GetTenantUsersAsync(tenantIdInexistente);

            // Assert
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetTenantUsersAsync_ComTenantSemUsuarios_DeveRetornarListaVazia()
        {
            // Arrange
            var tenant = new Tenant { NomeDaEmpresa = "Empresa Sem Usuários" };
            _context.Tenants.Add(tenant);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetTenantUsersAsync(tenant.Id);

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            
            // Para listas vazias, podemos verificar se é uma coleção enumerável vazia
            if (result.Data is System.Collections.IEnumerable enumerable)
            {
                var count = 0;
                foreach (var item in enumerable) count++;
                count.Should().Be(0);
            }
        }

        [Fact]
        public async Task GetTenantByIdAsync_ComTenantExistente_DeveRetornarTenant()
        {
            // Arrange
            var tenant = new Tenant 
            { 
                NomeDaEmpresa = "Empresa para Busca",
                StatusDaSubscricao = "Ativa",
                DataDeCriacao = DateTime.UtcNow
            };
            _context.Tenants.Add(tenant);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetTenantByIdAsync(tenant.Id);

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
        }

        [Fact]
        public async Task GetTenantByIdAsync_ComTenantInexistente_DeveRetornarErro()
        {
            // Arrange
            var tenantIdInexistente = Guid.NewGuid();

            // Act
            var result = await _service.GetTenantByIdAsync(tenantIdInexistente);

            // Assert
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().NotBeNullOrEmpty();
        }

        // Cleanup method para resetar o banco entre testes se necessário
        private async Task CleanDatabaseAsync()
        {
            _context.Users.RemoveRange(_context.Users);
            _context.Tenants.RemoveRange(_context.Tenants);
            await _context.SaveChangesAsync();
        }
    }
}