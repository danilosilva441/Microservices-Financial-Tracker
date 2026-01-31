using AuthService.DTO;
using AuthService.Models;
using SharedKernel;

namespace AuthService.Services.Interfaces
{
    public interface IUserService
    {
        // Alterado de LoginDto para CreateTenantUserDto para capturar o FullName no registro manual
        Task<AuthResult> RegisterAsync(CreateTenantUserDto request); 
        
        // Novo método necessário para o endpoint /me
        Task<AuthResult> GetUserByIdAsync(Guid userId); 

        Task<AuthResult> PromoteToAdminAsync(string userEmail);
        Task<AuthResult> DemoteFromAdminAsync(string userEmail);
        Task<AuthResult> GetAdminUsersAsync();
        Task<AuthResult> GetSubordinatesAsync(Guid managerId);
        Task<AuthResult> CreateTenantUserAsync(CreateTenantUserDto request, Guid managerUserId, Guid tenantId);
        Task<AuthResult> GetUserHierarchyAsync(Guid userId);

        Task<User> CreateSystemUserAsync(CreateSystemUserDto dto);
    }
}