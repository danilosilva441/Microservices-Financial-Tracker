using AuthService.Controllers;
using AuthService.DTO;
using AuthService.Services.Interfaces;
using AuthService.Models; // Para AuthResult
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace AuthService.Tests.Controllers
{
    public class TokenControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly TokenController _controller;

        public TokenControllerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _controller = new TokenController(_authServiceMock.Object);
        }

        [Fact]
        public async Task Login_CredenciaisValidas_DeveRetornarOkComToken()
        {
            // Arrange
            var request = new UserDto { Email = "teste@email.com", Password = "123" };
            var authResult = AuthResult.Ok(new { token = "jwt_fake" });

            _authServiceMock.Setup(s => s.LoginAsync(request))
                .ReturnsAsync(authResult);

            // Act
            var result = await _controller.Login(request);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task Login_CredenciaisInvalidas_DeveRetornarUnauthorized()
        {
            // Arrange
            var request = new UserDto { Email = "teste@email.com", Password = "errada" };
            var authResult = AuthResult.Fail("Credenciais inválidas.");

            _authServiceMock.Setup(s => s.LoginAsync(request))
                .ReturnsAsync(authResult);

            // Act
            var result = await _controller.Login(request);

            // Assert
            var unauthorizedResult = result.Should().BeOfType<UnauthorizedObjectResult>().Subject;
            unauthorizedResult.StatusCode.Should().Be(401);
            unauthorizedResult.Value.Should().Be("Credenciais inválidas.");
        }
    }
}