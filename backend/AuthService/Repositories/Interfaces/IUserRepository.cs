using AuthService.Models;

namespace AuthService.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByIdAsync(Guid userId);
        Task AddUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task<bool> UserExistsAsync(string email);
        Task<bool> UserExistsAsync(Guid userId);
        Task<List<User>> GetUsersByTenantAsync(Guid tenantId);
        Task<List<User>> GetSubordinatesAsync(Guid managerId);
        Task<bool> IsEmailUniqueAsync(string email, Guid? excludeUserId = null);
        Task<List<User>> GetAllUsersAsync();
        Task<List<User>> GetUsersByRoleAsync(string roleName);
    }
}