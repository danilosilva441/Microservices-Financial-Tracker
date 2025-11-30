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
    public class AdminControllerTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly AdminController _controller;

        public AdminControllerTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _controller = new AdminController(_userServiceMock.Object);
        }

        [Fact]
        public async Task PromoteToAdmin_Sucesso_DeveRetornarOk()
        {
            // Arrange
            var request = new PromoteAdminDto { Email = "user@teste.com" };
            _userServiceMock.Setup(s => s.PromoteToAdminAsync(request.Email))
                .ReturnsAsync(AuthResult.Ok("Sucesso"));

            // Act
            var result = await _controller.PromoteToAdmin(request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task PromoteToAdmin_UsuarioNaoEncontrado_DeveRetornarNotFound()
        {
            // Arrange
            var request = new PromoteAdminDto { Email = "inexistente@teste.com" };
            _userServiceMock.Setup(s => s.PromoteToAdminAsync(request.Email))
                .ReturnsAsync(AuthResult.Fail("Usuário não encontrado."));

            // Act
            var result = await _controller.PromoteToAdmin(request);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }
    }
}