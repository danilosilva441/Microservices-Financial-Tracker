using AuthService.Models;

namespace AuthService.Repositories
{
    // Define o contrato para operações de dados do Perfil
    public interface IRoleRepository
    {
        Task<Role?> GetRoleByNameAsync(string roleName);
    }
}