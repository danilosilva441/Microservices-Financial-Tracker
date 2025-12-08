using AuthService.Models;

namespace AuthService.Repositories
{
    public interface IRoleRepository
    {
        Task<Role?> GetRoleByNameAsync(string roleName);
        Task<Role?> GetRoleByIdAsync(Guid roleId);
        Task<List<Role>> GetAllRolesAsync();
        Task<List<Role>> GetRolesByNamesAsync(IEnumerable<string> roleNames);
        Task<bool> RoleExistsAsync(string roleName);
        Task<List<User>> GetUsersByRoleAsync(string roleName);
    }
}