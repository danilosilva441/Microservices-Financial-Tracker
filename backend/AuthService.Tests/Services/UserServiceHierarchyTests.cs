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

        private User CreateManagerUser(string roleName)
        {
            return new User 
            { 
                Id = Guid.NewGuid(), 
                Email = $"{roleName.ToLower()}@teste.com",
                Roles = new List<Role> 
                { 
                    new Role { Id = Guid.NewGuid(), Name = roleName, NormalizedName = roleName.ToUpper() }
                }
            };
        }

        [Fact]
        public async Task CreateTenantUserAsync_SupervisorTentaCriarSupervisor_DeveRetornarFalha()
        {
            // ARRANGE
            var supervisorId = Guid.NewGuid();
            var supervisorUser = CreateManagerUser("Supervisor");
            var tenantId = Guid.NewGuid();
            var request = new CreateTenantUserDto
            {
                Email = "novo.supervisor@teste.com",
                Password = "SenhaForte123!",
                RoleName = "Supervisor"
            };

            // Mock do supervisor
            _userRepositoryMock.Setup(r => r.GetUserByIdAsync(supervisorId))
                .ReturnsAsync(supervisorUser);

            // ACT
            var result = await _service.CreateTenantUserAsync(request, supervisorId, tenantId);

            // ASSERT
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Contain("Permissão negada");
        }

        [Fact]
        public async Task CreateTenantUserAsync_SupervisorTentaCriarLider_DeveSucesso()
        {
            // ARRANGE
            var supervisorId = Guid.NewGuid();
            var supervisorUser = CreateManagerUser("Supervisor");
            var tenantId = Guid.NewGuid();
            var request = new CreateTenantUserDto
            {
                Email = "lider@teste.com",
                Password = "SenhaForte123!",
                RoleName = "Lider"
            };

            // Mock do supervisor
            _userRepositoryMock.Setup(r => r.GetUserByIdAsync(supervisorId))
                .ReturnsAsync(supervisorUser);

            // Mock: Email não existe
            _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(request.Email))
                .ReturnsAsync((User?)null);

            // Mock: Role existe
            _roleRepositoryMock.Setup(r => r.GetRoleByNameAsync("Lider"))
                .ReturnsAsync(new Role { Id = Guid.NewGuid(), Name = "Lider", NormalizedName = "LIDER" });

            // Mock: Adição do usuário
            _userRepositoryMock.Setup(r => r.AddUserAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            // ACT
            var result = await _service.CreateTenantUserAsync(request, supervisorId, tenantId);

            // ASSERT
            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task CreateTenantUserAsync_LiderTentaCriarOperador_DeveSucesso()
        {
            // ARRANGE
            var liderId = Guid.NewGuid();
            var liderUser = CreateManagerUser("Lider");
            var tenantId = Guid.NewGuid();
            var request = new CreateTenantUserDto
            {
                Email = "operador@teste.com",
                Password = "SenhaForte123!",
                RoleName = "Operador"
            };

            // Mock do lider
            _userRepositoryMock.Setup(r => r.GetUserByIdAsync(liderId))
                .ReturnsAsync(liderUser);

            // Mock: Email não existe
            _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(request.Email))
                .ReturnsAsync((User?)null);

            // Mock: Role existe
            _roleRepositoryMock.Setup(r => r.GetRoleByNameAsync("Operador"))
                .ReturnsAsync(new Role { Id = Guid.NewGuid(), Name = "Operador", NormalizedName = "OPERADOR" });

            // Mock: Adição do usuário
            _userRepositoryMock.Setup(r => r.AddUserAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            // ACT
            var result = await _service.CreateTenantUserAsync(request, liderId, tenantId);

            // ASSERT
            result.Success.Should().BeTrue();
        }
    }
}