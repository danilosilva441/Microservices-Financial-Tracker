using AuthService.Models;

namespace AuthService.Repositories
{
    // Define o contrato para operações de dados do Usuário
    public interface IUserRepository
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task AddUserAsync(User user);
        Task UpdateUserAsync(User user); // Para salvar mudanças (ex: promover admin)
    }
}