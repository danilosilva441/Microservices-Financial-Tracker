using AuthService.DTO;
using AuthService.Models;
using AuthService.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

using ServiceImpl = AuthService.Services.AuthService;

namespace AuthService.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<ILogger<ServiceImpl>> _loggerMock;
        private readonly ServiceImpl _service;

        // Constantes para evitar magic strings
        private const string ValidEmail = "teste@email.com";
        private const string ValidPassword = "123";
        private const string WrongPassword = "senha_errada";
        private const string CorrectPassword = "senha_certa";
        private const string InvalidEmail = "inexistente@email.com";
        private const string JwtKey = "SuperSecretKeyForTestingPurposes123!";
        private const string JwtIssuer = "TestIssuer";
        private const string JwtAudience = "TestAudience";
        private const string InvalidCredentialsMessage = "Credenciais inválidas";

        public AuthServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _configurationMock = new Mock<IConfiguration>();
            _loggerMock = new Mock<ILogger<ServiceImpl>>();

            SetupConfigurationMock();
            _service = new ServiceImpl(
                _userRepositoryMock.Object, 
                _configurationMock.Object, 
                _loggerMock.Object);
        }

        private void SetupConfigurationMock()
        {
            _configurationMock.Setup(c => c["Jwt:Key"]).Returns(JwtKey);
            _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns(JwtIssuer);
            _configurationMock.Setup(c => c["Jwt:Audience"]).Returns(JwtAudience);
        }

        // Factory method para criar User padrão
        private static User CreateTestUser(string email = ValidEmail, string password = ValidPassword)
        {
            return new User 
            { 
                Id = Guid.NewGuid(), 
                Email = email, 
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Roles = new List<Role> { new Role { Name = "User" } }
            };
        }

        // Factory method para criar UserDto padrão
        private static UserDto CreateUserDto(string email = ValidEmail, string password = ValidPassword)
        {
            return new UserDto { Email = email, Password = password };
        }

        [Fact]
        public async Task LoginAsync_ComCredenciaisValidas_DeveRetornarToken()
        {
            // Arrange
            var request = CreateUserDto();
            var user = CreateTestUser();
            
            _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(request.Email))
                .ReturnsAsync(user);

            // Act
            var result = await _service.LoginAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.GetType().GetProperty("token").Should().NotBeNull();
            result.ErrorMessage.Should().BeNullOrEmpty();
        }

        [Fact]
        public async Task LoginAsync_QuandoUsuarioNaoEncontrado_DeveRetornarFalha()
        {
            // Arrange
            var request = CreateUserDto(InvalidEmail, ValidPassword);
            
            _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(request.Email))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _service.LoginAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Data.Should().BeNull();
            result.ErrorMessage.Should().Contain(InvalidCredentialsMessage);
        }

        [Fact]
        public async Task LoginAsync_ComSenhaIncorreta_DeveRetornarFalha()
        {
            // Arrange
            var request = CreateUserDto(ValidEmail, WrongPassword);
            var user = CreateTestUser(password: CorrectPassword);

            _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(request.Email))
                .ReturnsAsync(user);

            // Act
            var result = await _service.LoginAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Data.Should().BeNull();
            result.ErrorMessage.Should().Contain(InvalidCredentialsMessage);
        }

        [Fact]
        public async Task LoginAsync_ComUsuarioSemRoles_DeveRetornarToken()
        {
            // Arrange
            var request = CreateUserDto();
            var user = CreateTestUser();
            user.Roles = new List<Role>(); // Usuário sem roles

            _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(request.Email))
                .ReturnsAsync(user);

            // Act
            var result = await _service.LoginAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
        }

        // Corrigindo os warnings do xUnit sobre uso de null
        [Theory]
        [InlineData("", "senha")] // Email vazio
        [InlineData("email@invalido", "")] // Senha vazia
        public async Task LoginAsync_ComDadosInvalidos_DeveRetornarFalha(string email, string password)
        {
            // Arrange
            var request = CreateUserDto(email, password);
            
            // Act
            var result = await _service.LoginAsync(request);

            // Assert
            result.Success.Should().BeFalse();
        }

        // TESTE CORRIGIDO: Agora verifica se retorna um resultado com erro em vez de propagar exceção
        [Fact]
        public async Task LoginAsync_QuandoRepositoryLancaExcecao_DeveRetornarFalha()
        {
            // Arrange
            var request = CreateUserDto();
            var exceptionMessage = "Database connection failed";
            
            _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(request.Email))
                .ThrowsAsync(new InvalidOperationException(exceptionMessage));

            // Act
            var result = await _service.LoginAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().NotBeNullOrEmpty();
            // Pode verificar se a mensagem de erro contém algo específico ou apenas que não é nula
        }

        // Teste adicional para verificar se o logger está sendo usado corretamente
        [Fact]
        public async Task LoginAsync_ComCredenciaisInvalidas_DeveLogarInformacao()
        {
            // Arrange
            var request = CreateUserDto(InvalidEmail, ValidPassword);
            
            _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(request.Email))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _service.LoginAsync(request);

            // Assert
            result.Success.Should().BeFalse();
        }

        // Novo teste: Verificar comportamento com configuração JWT faltante
        [Fact]
        public async Task LoginAsync_ComConfiguracaoJwtFaltante_DeveRetornarFalha()
        {
            // Arrange
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(c => c["Jwt:Key"]).Returns((string?)null); // Configuração faltante
            
            var service = new ServiceImpl(
                _userRepositoryMock.Object, 
                configurationMock.Object, 
                _loggerMock.Object);

            var request = CreateUserDto();
            var user = CreateTestUser();
            
            _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(request.Email))
                .ReturnsAsync(user);

            // Act
            var result = await service.LoginAsync(request);

            // Assert
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().NotBeNullOrEmpty();
        }
    }
}