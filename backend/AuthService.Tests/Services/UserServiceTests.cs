using AuthService.DTO;
using AuthService.Models;
using AuthService.Repositories;
using AuthService.Services;
using FluentAssertions;
using Moq;
using Xunit;
using Microsoft.Extensions.Logging;

namespace AuthService.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IRoleRepository> _roleRepositoryMock;
        private readonly Mock<ILogger<UserService>> _loggerMock;
        private readonly UserService _service;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _roleRepositoryMock = new Mock<IRoleRepository>();
            _loggerMock = new Mock<ILogger<UserService>>();
            _service = new UserService(_userRepositoryMock.Object, _roleRepositoryMock.Object, _loggerMock.Object);
        }

        private User CreateManagerUser(string roleName = "Gerente")
        {
            return new User
            {
                Id = Guid.NewGuid(),
                Email = "manager@teste.com",
                Roles = new List<Role>
                {
                    new Role { Id = Guid.NewGuid(), Name = roleName, NormalizedName = roleName.ToUpper() }
                }
            };
        }

        private void SetupCommonMocks(Guid managerId, User managerUser, string userEmail, Role? role = null)
        {
            // Mock do manager - ESSENCIAL E ESTAVA FALTANDO!
            _userRepositoryMock.Setup(r => r.GetUserByIdAsync(managerId))
                .ReturnsAsync(managerUser);

            // Mock: Email não existe
            _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(userEmail))
                .ReturnsAsync((User?)null);

            // Mock: Role se fornecida
            if (role != null)
            {
                _roleRepositoryMock.Setup(r => r.GetRoleByNameAsync(role.Name))
                    .ReturnsAsync(role);
            }
        }

        [Fact]
        public async Task CreateTenantUserAsync_GerenteTentaCriarGerente_DeveRetornarFalha()
        {
            // ARRANGE
            var managerId = Guid.NewGuid();
            var managerUser = CreateManagerUser("Gerente");
            var tenantId = Guid.NewGuid();
            var request = new CreateTenantUserDto
            {
                Email = "novo.gerente@teste.com",
                Password = "SenhaForte123!",
                RoleName = "Gerente"
            };

            SetupCommonMocks(managerId, managerUser, request.Email);

            // ACT
            var result = await _service.CreateTenantUserAsync(request, managerId, tenantId);

            // ASSERT
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Contain("Permissão negada");
        }

        [Fact]
        public async Task CreateTenantUserAsync_EmailJaExistente_DeveRetornarFalha()
        {
            // ARRANGE
            var managerId = Guid.NewGuid();
            var managerUser = CreateManagerUser("Gerente");
            var tenantId = Guid.NewGuid();
            var request = new CreateTenantUserDto
            {
                Email = "email.existente@teste.com",
                Password = "SenhaForte123!",
                RoleName = "Operador"
            };

            // Mock do manager
            _userRepositoryMock.Setup(r => r.GetUserByIdAsync(managerId))
                .ReturnsAsync(managerUser);

            // CORREÇÃO: Mock do UserExistsAsync (não do GetUserByEmailAsync)
            _userRepositoryMock.Setup(r => r.UserExistsAsync(request.Email.Trim().ToLower()))
                .ReturnsAsync(true); // ← Email já existe

            // Mock da role (ainda necessário para outras validações)
            _roleRepositoryMock.Setup(r => r.GetRoleByNameAsync("Operador"))
                .ReturnsAsync(new Role { Id = Guid.NewGuid(), Name = "Operador", NormalizedName = "OPERADOR" });

            // ACT
            var result = await _service.CreateTenantUserAsync(request, managerId, tenantId);

            // ASSERT
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Contain("já existe");
        }

        [Fact]
        public async Task CreateTenantUserAsync_RoleNaoExistente_DeveRetornarFalha()
        {
            // ARRANGE
            var managerId = Guid.NewGuid();
            var managerUser = CreateManagerUser("Gerente");
            var tenantId = Guid.NewGuid();
            var request = new CreateTenantUserDto
            {
                Email = "usuario@teste.com",
                Password = "SenhaForte123!",
                RoleName = "RoleInexistente"
            };

            SetupCommonMocks(managerId, managerUser, request.Email);

            // Mock: Role não existe
            _roleRepositoryMock.Setup(r => r.GetRoleByNameAsync("RoleInexistente"))
                .ReturnsAsync((Role?)null);

            // ACT
            var result = await _service.CreateTenantUserAsync(request, managerId, tenantId);

            // ASSERT
            result.Success.Should().BeFalse();
            // CORREÇÃO: A mensagem real é "Perfil 'RoleInexistente' não é válido."
            result.ErrorMessage.Should().Contain("RoleInexistente");
        }

        [Fact]
        public async Task CreateTenantUserAsync_GerenteTentaCriarAdmin_DeveRetornarFalha()
        {
            // ARRANGE
            var managerId = Guid.NewGuid();
            var managerUser = CreateManagerUser("Gerente");
            var tenantId = Guid.NewGuid();
            var request = new CreateTenantUserDto
            {
                Email = "admin@teste.com",
                Password = "SenhaForte123!",
                RoleName = "Admin"
            };

            SetupCommonMocks(managerId, managerUser, request.Email);

            // ACT
            var result = await _service.CreateTenantUserAsync(request, managerId, tenantId);

            // ASSERT
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Contain("Permissão negada");
        }

        [Fact]
        public async Task CreateTenantUserAsync_GerenteTentaCriarDev_DeveRetornarFalha()
        {
            // ARRANGE
            var managerId = Guid.NewGuid();
            var managerUser = CreateManagerUser("Gerente");
            var tenantId = Guid.NewGuid();
            var request = new CreateTenantUserDto
            {
                Email = "dev@teste.com",
                Password = "SenhaForte123!",
                RoleName = "Dev"
            };

            SetupCommonMocks(managerId, managerUser, request.Email);

            // ACT
            var result = await _service.CreateTenantUserAsync(request, managerId, tenantId);

            // ASSERT
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Contain("Permissão negada");
        }

        [Fact]
        public async Task CreateTenantUserAsync_GerenteTentaCriarOperador_DeveSucesso()
        {
            // ARRANGE
            var managerId = Guid.NewGuid();
            var managerUser = CreateManagerUser("Gerente");
            var tenantId = Guid.NewGuid();
            var request = new CreateTenantUserDto
            {
                Email = "operador@teste.com",
                Password = "SenhaForte123!",
                RoleName = "Operador"
            };

            var role = new Role { Id = Guid.NewGuid(), Name = "Operador", NormalizedName = "OPERADOR" };
            SetupCommonMocks(managerId, managerUser, request.Email, role);

            // Mock: Adição do usuário
            _userRepositoryMock.Setup(r => r.AddUserAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            // ACT
            var result = await _service.CreateTenantUserAsync(request, managerId, tenantId);

            // ASSERT
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateTenantUserAsync_GerenteTentaCriarLider_DeveSucesso()
        {
            // ARRANGE
            var managerId = Guid.NewGuid();
            var managerUser = CreateManagerUser("Gerente");
            var tenantId = Guid.NewGuid();
            var request = new CreateTenantUserDto
            {
                Email = "lider@teste.com",
                Password = "SenhaForte123!",
                RoleName = "Lider"
            };

            var role = new Role { Id = Guid.NewGuid(), Name = "Lider", NormalizedName = "LIDER" };
            SetupCommonMocks(managerId, managerUser, request.Email, role);

            // Mock: Adição do usuário
            _userRepositoryMock.Setup(r => r.AddUserAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            // ACT
            var result = await _service.CreateTenantUserAsync(request, managerId, tenantId);

            // ASSERT
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateTenantUserAsync_GerenteTentaCriarUser_DeveSucesso()
        {
            // ARRANGE
            var managerId = Guid.NewGuid();
            var managerUser = CreateManagerUser("Gerente");
            var tenantId = Guid.NewGuid();
            var request = new CreateTenantUserDto
            {
                Email = "user@teste.com",
                Password = "SenhaForte123!",
                RoleName = "User"
            };

            var role = new Role { Id = Guid.NewGuid(), Name = "User", NormalizedName = "USER" };
            SetupCommonMocks(managerId, managerUser, request.Email, role);

            // Mock: Adição do usuário
            _userRepositoryMock.Setup(r => r.AddUserAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            // ACT
            var result = await _service.CreateTenantUserAsync(request, managerId, tenantId);

            // ASSERT
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateTenantUserAsync_SenhaFraca_DeveRetornarFalha()
        {
            // ARRANGE
            var managerId = Guid.NewGuid();
            var managerUser = CreateManagerUser("Gerente");
            var tenantId = Guid.NewGuid();
            var request = new CreateTenantUserDto
            {
                Email = "usuario@teste.com",
                Password = "123", // Senha fraca
                RoleName = "Operador"
            };

            SetupCommonMocks(managerId, managerUser, request.Email);

            // ACT
            var result = await _service.CreateTenantUserAsync(request, managerId, tenantId);

            // ASSERT
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Contain("senha");
        }

        [Theory]
        [InlineData("Admin")]
        [InlineData("Dev")]
        [InlineData("Gerente")]
        public async Task CreateTenantUserAsync_GerenteNaoPodeCriarRolesSuperioresOuIguais_DeveFalhar(string roleName)
        {
            // ARRANGE
            var managerId = Guid.NewGuid();
            var managerUser = CreateManagerUser("Gerente");
            var tenantId = Guid.NewGuid();
            var request = new CreateTenantUserDto
            {
                Email = $"{roleName.ToLower()}@teste.com",
                Password = "SenhaForte123!",
                RoleName = roleName
            };

            SetupCommonMocks(managerId, managerUser, request.Email);

            // ACT
            var result = await _service.CreateTenantUserAsync(request, managerId, tenantId);

            // ASSERT
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Contain("Permissão negada");
        }

        [Theory]
        [InlineData("Supervisor")]
        [InlineData("Lider")]
        [InlineData("Operador")]
        [InlineData("User")]
        public async Task CreateTenantUserAsync_GerentePodeCriarRolesInferiores_DeveSucesso(string roleName)
        {
            // ARRANGE
            var managerId = Guid.NewGuid();
            var managerUser = CreateManagerUser("Gerente");
            var tenantId = Guid.NewGuid();
            var request = new CreateTenantUserDto
            {
                Email = $"{roleName.ToLower()}@teste.com",
                Password = "SenhaForte123!",
                RoleName = roleName
            };

            var role = new Role { Id = Guid.NewGuid(), Name = roleName, NormalizedName = roleName.ToUpper() };
            SetupCommonMocks(managerId, managerUser, request.Email, role);

            // Mock: Adição do usuário
            _userRepositoryMock.Setup(r => r.AddUserAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            // ACT
            var result = await _service.CreateTenantUserAsync(request, managerId, tenantId);

            // ASSERT
            result.Success.Should().BeTrue($"{roleName} deve ser permitido para Gerente");
            result.Data.Should().NotBeNull();
        }
    }
}