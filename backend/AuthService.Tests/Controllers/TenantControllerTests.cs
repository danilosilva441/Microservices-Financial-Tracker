using AuthService.Controllers;
using AuthService.DTO;
using AuthService.Models;
using AuthService.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace AuthService.Tests.Controllers
{
    public class TenantControllerTests
    {
        private readonly Mock<ITenantService> _tenantServiceMock;
        private readonly TenantController _controller;

        public TenantControllerTests()
        {
            _tenantServiceMock = new Mock<ITenantService>();
            _controller = new TenantController(_tenantServiceMock.Object);
        }

        [Fact]
        public async Task ProvisionTenant_Sucesso_DeveRetornarCreated()
        {
            // Arrange
            var request = new TenantProvisionDto { NomeDaEmpresa = "Empresa", EmailDoGerente = "g@g.com" };
            _tenantServiceMock.Setup(s => s.ProvisionTenantAsync(request))
                .ReturnsAsync(AuthResult.Ok(new { TenantId = Guid.NewGuid() }));

            // Act
            var result = await _controller.ProvisionTenant(request);

            // Assert
            var createdResult = result.Should().BeOfType<ObjectResult>().Subject;
            createdResult.StatusCode.Should().Be(201);
        }

        [Fact]
        public async Task ProvisionTenant_UsuarioJaExiste_DeveRetornarBadRequest()
        {
            // Arrange
            var request = new TenantProvisionDto();
            _tenantServiceMock.Setup(s => s.ProvisionTenantAsync(request))
                .ReturnsAsync(AuthResult.Fail("Um usuário com este email já existe."));

            // Act
            var result = await _controller.ProvisionTenant(request);

            // Assert
            var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequest.Value.Should().Be("Um usuário com este email já existe.");
        }

        [Fact]
        public async Task ProvisionTenant_ErroInterno_DeveRetornar500()
        {
            // Arrange
            var request = new TenantProvisionDto();
            _tenantServiceMock.Setup(s => s.ProvisionTenantAsync(request))
                .ReturnsAsync(AuthResult.Fail("Erro crítico no banco."));

            // Act
            var result = await _controller.ProvisionTenant(request);

            // Assert
            var serverError = result.Should().BeOfType<ObjectResult>().Subject;
            serverError.StatusCode.Should().Be(500);
        }
    }
}