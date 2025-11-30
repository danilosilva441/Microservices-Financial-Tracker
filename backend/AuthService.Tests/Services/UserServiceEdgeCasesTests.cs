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
    public class UserServiceEdgeCasesTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IRoleRepository> _roleRepositoryMock;
        private readonly Mock<ILogger<UserService>> _loggerMock;
        private readonly UserService _service;

        public UserServiceEdgeCasesTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _roleRepositoryMock = new Mock<IRoleRepository>();
            _loggerMock = new Mock<ILogger<UserService>>();
            _service = new UserService(_userRepositoryMock.Object, _roleRepositoryMock.Object, _loggerMock.Object);
        }

        private User CreateManagerUser()
        {
            return new User 
            { 
                Id = Guid.NewGuid(), 
                Email = "manager@teste.com",
                Roles = new List<Role> 
                { 
                    new Role { Id = Guid.NewGuid(), Name = "Gerente", NormalizedName = "GERENTE" }
                }
            };
        }

        [Fact]
        public async Task CreateTenantUserAsync_EmailInvalido_DeveRetornarFalha()
        {
            // ARRANGE
            var managerId = Guid.NewGuid();
            var managerUser = CreateManagerUser();
            var tenantId = Guid.NewGuid();
            var request = new CreateTenantUserDto
            {
                Email = "email-invalido",
                Password = "SenhaForte123!",
                RoleName = "Operador"
            };

            // Mock do manager
            _userRepositoryMock.Setup(r => r.GetUserByIdAsync(managerId))
                .ReturnsAsync(managerUser);

            // ACT
            var result = await _service.CreateTenantUserAsync(request, managerId, tenantId);

            // ASSERT
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Contain("email");
        }

        [Fact]
        public async Task CreateTenantUserAsync_DadosObrigatoriosFaltando_DeveRetornarFalha()
        {
            // ARRANGE
            var managerId = Guid.NewGuid();
            var managerUser = CreateManagerUser();
            var tenantId = Guid.NewGuid();
            var request = new CreateTenantUserDto
            {
                Email = "",
                Password = "",
                RoleName = ""
            };

            // Mock do manager
            _userRepositoryMock.Setup(r => r.GetUserByIdAsync(managerId))
                .ReturnsAsync(managerUser);

            // ACT
            var result = await _service.CreateTenantUserAsync(request, managerId, tenantId);

            // ASSERT
            result.Success.Should().BeFalse();
        }

        [Fact]
        public async Task CreateTenantUserAsync_RepositoryException_DeveRetornarFalha()
        {
            // ARRANGE
            var managerId = Guid.NewGuid();
            var managerUser = CreateManagerUser();
            var tenantId = Guid.NewGuid();
            var request = new CreateTenantUserDto
            {
                Email = "teste@teste.com",
                Password = "SenhaForte123!",
                RoleName = "Operador"
            };

            // Mock do manager
            _userRepositoryMock.Setup(r => r.GetUserByIdAsync(managerId))
                .ReturnsAsync(managerUser);

            // Mock da role
            _roleRepositoryMock.Setup(r => r.GetRoleByNameAsync("Operador"))
                .ReturnsAsync(new Role { Id = Guid.NewGuid(), Name = "Operador", NormalizedName = "OPERADOR" });

            // CORREÇÃO: Simula exceção na ADIÇÃO do usuário (não na verificação de email)
            _userRepositoryMock.Setup(r => r.AddUserAsync(It.IsAny<User>()))
                .ThrowsAsync(new Exception("Erro de banco de dados"));

            // ACT
            var result = await _service.CreateTenantUserAsync(request, managerId, tenantId);

            // ASSERT
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Contain("Erro");
        }
    }
}