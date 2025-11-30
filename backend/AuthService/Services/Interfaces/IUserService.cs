// Caminho: backend/AuthService/Services/Interfaces/IUserService.cs
using AuthService.DTO;
using SharedKernel;

namespace AuthService.Services.Interfaces
{
    public interface IUserService
    {
        Task<AuthResult> RegisterAsync(UserDto request);
        Task<AuthResult> PromoteToAdminAsync(string userEmail);
        Task<AuthResult> DemoteFromAdminAsync(string userEmail);
        Task<AuthResult> GetAdminUsersAsync();
        Task<AuthResult> CreateTenantUserAsync(CreateTenantUserDto request, Guid managerUserId, Guid tenantId);
        Task<AuthResult> GetUserHierarchyAsync(Guid userId);
    }
}