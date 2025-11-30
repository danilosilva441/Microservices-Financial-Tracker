using AuthService.DTO;
using SharedKernel;

namespace AuthService.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResult> LoginAsync(UserDto request);
        Task<AuthResult> ValidateTokenAsync(string token);
        Task<AuthResult> RefreshTokenAsync(string expiredToken);
    }
}