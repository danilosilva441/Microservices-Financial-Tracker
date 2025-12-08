using AuthService.DTO;
using AuthService.Models;
using AuthService.Repositories;
using AuthService.Services;
using FluentAssertions;
using Moq;
using SharedKernel;
using Xunit;
using Microsoft.Extensions.Logging;

namespace AuthService.Tests.Services
{
    public class UserServiceHierarchyTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IRoleRepository> _roleRepositoryMock;
        private readonly Mock<ILogger<UserService>> _loggerMock;
        private readonly UserService _service;

        public UserServiceHierarchyTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _roleRepositoryMock = new Mock<IRoleRepository>();
            _loggerMock = new Mock<ILogger<UserService>>();
            _service = new UserService(_userRepositoryMock.Object, _roleRepositoryMock.Object, _loggerMock.Object);
        }

        // Constantes para evitar "magic strings" - usando as mensagens REAIS da implementação
        private const string SupervisorRole = "Supervisor";
        private const string LiderRole = "Lider";
        private const string OperadorRole = "Operador";

        private const string StrongPassword = "SenhaForte123!";

        // Mensagens de erro REAIS da implementação (extraídas dos logs de erro)
        private const string PermissionDeniedMessage = "Permissão negada. Você só pode criar usuários com perfis de nível inferior ao seu.";
        private const string ManagerNotFoundMessage = "Manager não encontrado.";
        private const string EmailInUseMessage = "Um usuario com este email já existe.";
        private const string RoleNotFoundMessage = "Perfil '{0}' não é válido.";
        private const string ManagerNoPermissionMessage = "Manager não possui permissões válidas.";

        // Factory method para criar usuários com roles
        private User CreateUserWithRole(Guid? id = null, string roleName = SupervisorRole, string? email = null)
        {
            var userId = id ?? Guid.NewGuid();
            var userEmail = email ?? $"{roleName.ToLower()}@teste.com";

            return new User
            {
                Id = userId,
                Email = userEmail,
                TenantId = Guid.NewGuid(), // Adicionado TenantId
                Roles = new List<Role>
                {
                    new Role
                    {
                        Id = Guid.NewGuid(),
                        Name = roleName,
                        NormalizedName = roleName.ToUpper()
                    }
                }
            };
        }

        // Helper para setup comum
        private void SetupUserExists(Guid userId, string roleName)
        {
            var user = CreateUserWithRole(userId, roleName);
            _userRepositoryMock.Setup(r => r.GetUserByIdAsync(userId))
                .ReturnsAsync(user);
        }

        private void SetupRoleExists(string roleName)
        {
            _roleRepositoryMock.Setup(r => r.GetRoleByNameAsync(roleName))
                .ReturnsAsync(new Role
                {
                    Id = Guid.NewGuid(),
                    Name = roleName,
                    NormalizedName = roleName.ToUpper()
                });
        }

        private void SetupEmailNotExists(string email)
        {
            _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(email))
                .ReturnsAsync((User?)null);
        }

        [Theory]
        [InlineData(SupervisorRole, SupervisorRole, false, PermissionDeniedMessage)]
        [InlineData(SupervisorRole, LiderRole, true, null)]
        [InlineData(SupervisorRole, OperadorRole, true, null)]
        [InlineData(LiderRole, LiderRole, false, PermissionDeniedMessage)]
        [InlineData(LiderRole, OperadorRole, true, null)]
        [InlineData(OperadorRole, OperadorRole, false, PermissionDeniedMessage)]
        [InlineData(OperadorRole, LiderRole, false, PermissionDeniedMessage)]
        [InlineData(OperadorRole, SupervisorRole, false, PermissionDeniedMessage)]
        public async Task CreateTenantUserAsync_DeveRespeitarHierarquiaDeRoles(
            string creatorRole,
            string targetRole,
            bool expectedSuccess,
            string? expectedErrorMessage)
        {
            // ARRANGE
            var creatorId = Guid.NewGuid();
            var tenantId = Guid.NewGuid();
            var request = new CreateTenantUserDto
            {
                Email = $"{targetRole.ToLower()}@teste.com",
                Password = StrongPassword,
                RoleName = targetRole
            };

            // Setup criador
            SetupUserExists(creatorId, creatorRole);

            if (expectedSuccess)
            {
                // Setup para cenário de sucesso
                SetupEmailNotExists(request.Email);
                SetupRoleExists(targetRole);

                _userRepositoryMock.Setup(r => r.AddUserAsync(It.IsAny<User>()))
                    .Returns(Task.CompletedTask)
                    .Verifiable();
            }

            // ACT
            var result = await _service.CreateTenantUserAsync(request, creatorId, tenantId);

            // ASSERT
            result.Success.Should().Be(expectedSuccess);

            if (!expectedSuccess)
            {
                result.ErrorMessage.Should().Be(expectedErrorMessage!);
                _userRepositoryMock.Verify(r => r.AddUserAsync(It.IsAny<User>()), Times.Never);
            }
            else
            {
                result.ErrorMessage.Should().BeNullOrEmpty();
                _userRepositoryMock.Verify(r => r.AddUserAsync(It.IsAny<User>()), Times.Once);
            }
        }

        [Fact]
        public async Task CreateTenantUserAsync_QuandoCriadorNaoExiste_DeveRetornarFalha()
        {
            // ARRANGE
            var creatorId = Guid.NewGuid();
            var tenantId = Guid.NewGuid();
            var request = new CreateTenantUserDto
            {
                Email = "teste@teste.com",
                Password = StrongPassword,
                RoleName = OperadorRole
            };

            _userRepositoryMock.Setup(r => r.GetUserByIdAsync(creatorId))
                .ReturnsAsync((User?)null);

            // ACT
            var result = await _service.CreateTenantUserAsync(request, creatorId, tenantId);

            // ASSERT
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be(ManagerNotFoundMessage);
            _userRepositoryMock.Verify(r => r.AddUserAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task CreateTenantUserAsync_QuandoEmailJaExiste_DeveRetornarFalha()
        {
            // Arrange
            var managerId = Guid.NewGuid();
            var tenantId = Guid.NewGuid();
            var request = new CreateTenantUserDto
            {
                Email = "existente@teste.com",
                RoleName = "Operador",
                Password = "SenhaForte123!"
            };

            // Mock: Manager existe e tem permissão (para passar na validação de hierarquia)
            var managerUser = new User
            {
                Id = managerId,
                Roles = new List<Role> { new Role { Name = "Gerente" } }
            };
            _userRepositoryMock.Setup(r => r.GetUserByIdAsync(managerId)).ReturnsAsync(managerUser);

            // Mock: Role existe
            _roleRepositoryMock.Setup(r => r.GetRoleByNameAsync("Operador"))
                .ReturnsAsync(new Role { Name = "Operador" });

            // Mock: Email JÁ EXISTE (O ponto crucial)
            // Usamos It.IsAny<string>() para garantir que o mock responda "true"
            // independentemente de como o serviço normaliza a string (Trim/ToLower).
            _userRepositoryMock.Setup(r => r.UserExistsAsync(It.Is<string>(s => s == request.Email.ToLower())))
                .ReturnsAsync(true);

            // Act
            var result = await _service.CreateTenantUserAsync(request, managerId, tenantId);

            // Assert
            result.Success.Should().BeFalse("email duplicado deve impedir criação");
            result.ErrorMessage.Should().Contain(ErrorMessages.EmailInUse);

            // Verifica que NÃO chamou o AddUser
            _userRepositoryMock.Verify(r => r.AddUserAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task CreateTenantUserAsync_QuandoRoleNaoExiste_DeveRetornarFalha()
        {
            // ARRANGE
            var creatorId = Guid.NewGuid();
            var tenantId = Guid.NewGuid();
            var nonExistentRole = "RoleInexistente";
            var request = new CreateTenantUserDto
            {
                Email = "teste@teste.com",
                Password = StrongPassword,
                RoleName = nonExistentRole
            };

            SetupUserExists(creatorId, SupervisorRole);
            SetupEmailNotExists(request.Email);

            _roleRepositoryMock.Setup(r => r.GetRoleByNameAsync(nonExistentRole))
                .ReturnsAsync((Role?)null);

            // ACT
            var result = await _service.CreateTenantUserAsync(request, creatorId, tenantId);

            // ASSERT
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be(string.Format(RoleNotFoundMessage, nonExistentRole));
            _userRepositoryMock.Verify(r => r.AddUserAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task CreateTenantUserAsync_QuandoUsuarioCriado_DeveTerTenantIdCorreto()
        {
            // ARRANGE
            var creatorId = Guid.NewGuid();
            var tenantId = Guid.NewGuid();
            var request = new CreateTenantUserDto
            {
                Email = "novo@teste.com",
                Password = StrongPassword,
                RoleName = OperadorRole
            };

            SetupUserExists(creatorId, SupervisorRole);
            SetupEmailNotExists(request.Email);
            SetupRoleExists(OperadorRole);

            User? createdUser = null;
            _userRepositoryMock.Setup(r => r.AddUserAsync(It.IsAny<User>()))
                .Callback<User>(user => createdUser = user)
                .Returns(Task.CompletedTask);

            // ACT
            var result = await _service.CreateTenantUserAsync(request, creatorId, tenantId);

            // ASSERT
            result.Success.Should().BeTrue();
            createdUser.Should().NotBeNull();
            createdUser!.TenantId.Should().Be(tenantId);
            createdUser.Email.Should().Be(request.Email);
            createdUser.Roles.Should().ContainSingle(r => r.Name == OperadorRole);
        }

        [Fact]
        public async Task CreateTenantUserAsync_QuandoCriadorSemRole_DeveRetornarFalha()
        {
            // ARRANGE
            var creatorId = Guid.NewGuid();
            var tenantId = Guid.NewGuid();
            var request = new CreateTenantUserDto
            {
                Email = "teste@teste.com",
                Password = StrongPassword,
                RoleName = OperadorRole
            };

            var userWithoutRole = new User
            {
                Id = creatorId,
                Email = "semrole@teste.com",
                TenantId = Guid.NewGuid(), // Adicionado TenantId
                Roles = new List<Role>() // Lista vazia
            };

            _userRepositoryMock.Setup(r => r.GetUserByIdAsync(creatorId))
                .ReturnsAsync(userWithoutRole);

            // ACT
            var result = await _service.CreateTenantUserAsync(request, creatorId, tenantId);

            // ASSERT
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be(ManagerNoPermissionMessage);
            _userRepositoryMock.Verify(r => r.AddUserAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task CreateTenantUserAsync_QuandoRoleInvalida_DeveRetornarFalha()
        {
            // ARRANGE
            var creatorId = Guid.NewGuid();
            var tenantId = Guid.NewGuid();
            var invalidRole = "Admin"; // Role que não está na hierarquia permitida
            var request = new CreateTenantUserDto
            {
                Email = "admin@teste.com",
                Password = StrongPassword,
                RoleName = invalidRole
            };

            SetupUserExists(creatorId, SupervisorRole);
            SetupEmailNotExists(request.Email);

            // Setup: Role existe no banco mas não é válida para criação
            _roleRepositoryMock.Setup(r => r.GetRoleByNameAsync(invalidRole))
                .ReturnsAsync(new Role { Name = invalidRole });

            // ACT
            var result = await _service.CreateTenantUserAsync(request, creatorId, tenantId);

            // ASSERT
            result.Success.Should().BeFalse();
            // A mensagem real parece ser a de permissão negada hierárquica
            result.ErrorMessage.Should().Be(PermissionDeniedMessage);
            _userRepositoryMock.Verify(r => r.AddUserAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task CreateTenantUserAsync_QuandoUsuarioCriado_PasswordDeveSerHashed()
        {
            // ARRANGE
            var creatorId = Guid.NewGuid();
            var tenantId = Guid.NewGuid();
            var password = "SenhaForte123!";
            var request = new CreateTenantUserDto
            {
                Email = "novo@teste.com",
                Password = password,
                RoleName = OperadorRole
            };

            SetupUserExists(creatorId, SupervisorRole);
            SetupEmailNotExists(request.Email);
            SetupRoleExists(OperadorRole);

            User? createdUser = null;
            _userRepositoryMock.Setup(r => r.AddUserAsync(It.IsAny<User>()))
                .Callback<User>(user => createdUser = user)
                .Returns(Task.CompletedTask);

            // ACT
            var result = await _service.CreateTenantUserAsync(request, creatorId, tenantId);

            // ASSERT
            result.Success.Should().BeTrue();
            createdUser.Should().NotBeNull();
            createdUser!.PasswordHash.Should().NotBeNullOrEmpty();
            // O hash não deve ser igual à senha em texto claro
            createdUser.PasswordHash.Should().NotBe(password);
            // Corrigido: verificar o comprimento do hash
            createdUser.PasswordHash.Length.Should().BeGreaterThan(20);
        }
    }
}