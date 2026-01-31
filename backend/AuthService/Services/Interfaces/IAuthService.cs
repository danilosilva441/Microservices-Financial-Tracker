using AuthService.DTO;
using SharedKernel;

namespace AuthService.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResult> LoginAsync(LoginDto request);
        Task<AuthResult> ValidateTokenAsync(string token);
        Task<AuthResult> RefreshTokenAsync(string expiredToken);
    }
}