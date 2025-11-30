using AuthService.Controllers;
using AuthService.DTO;
using AuthService.Models;
using AuthService.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;

namespace AuthService.Tests.Controllers
{
    public class UsersControllerTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _controller = new UsersController(_userServiceMock.Object);
        }

        // --- Testes de Registro Público ---

        [Fact]
        public async Task Register_Sucesso_DeveRetornarCreated()
        {
            var request = new UserDto { Email = "dev@teste.com", Password = "123" };
            _userServiceMock.Setup(s => s.RegisterAsync(request))
                .ReturnsAsync(AuthResult.Ok(new { Id = Guid.NewGuid() }));

            var result = await _controller.Register(request);

            var createdResult = result.Should().BeOfType<ObjectResult>().Subject;
            createdResult.StatusCode.Should().Be(201);
        }

        // --- Testes de Hierarquia (CreateTenantUser) ---
        // Aqui precisamos simular o Token JWT

        [Fact]
        public async Task CreateTenantUser_Sucesso_DeveRetornarCreated()
        {
            // Arrange
            var request = new CreateTenantUserDto { Email = "sub@teste.com", RoleName = "Lider" };
            var managerId = Guid.NewGuid();
            var tenantId = Guid.NewGuid();

            // 1. Simular o Contexto HTTP (O Token)
            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, managerId.ToString()),
                new Claim("tenantId", tenantId.ToString())
            }));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userClaims }
            };

            // 2. Configurar o Mock do Serviço
            _userServiceMock.Setup(s => s.CreateTenantUserAsync(request, managerId, tenantId))
                .ReturnsAsync(AuthResult.Ok(new { Id = Guid.NewGuid() }));

            // Act
            var result = await _controller.CreateTenantUser(request);

            // Assert
            var createdResult = result.Should().BeOfType<ObjectResult>().Subject;
            createdResult.StatusCode.Should().Be(201);
        }

        [Fact]
        public async Task CreateTenantUser_PermissaoNegada_DeveRetornarForbidden()
        {
            // Arrange
            var request = new CreateTenantUserDto { RoleName = "Gerente" };
            
            // Simular Token
            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new Claim("tenantId", Guid.NewGuid().ToString())
            }));
            _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = userClaims } };

            // O Serviço retorna erro de permissão
            _userServiceMock.Setup(s => s.CreateTenantUserAsync(It.IsAny<CreateTenantUserDto>(), It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(AuthResult.Fail("Permissão negada."));

            // Act
            var result = await _controller.CreateTenantUser(request);

            // Assert
            var forbidden = result.Should().BeOfType<ObjectResult>().Subject;
            forbidden.StatusCode.Should().Be(403); // 403 Forbidden
        }
    }
}