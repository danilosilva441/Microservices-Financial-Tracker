// Caminho: backend/AuthService/Services/Interfaces/IAuthService.cs
using AuthService.DTO;

namespace AuthService.Services.Interfaces
{
    // A interface agora só tem o Login
    public interface IAuthService
    {
        Task<AuthResult> LoginAsync(UserDto request);
    }
}