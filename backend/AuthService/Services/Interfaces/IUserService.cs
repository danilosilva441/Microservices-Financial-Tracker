// Caminho: backend/AuthService/Services/Interfaces/IUserService.cs
using AuthService.DTO;

namespace AuthService.Services.Interfaces
{
    public interface IUserService
    {
        // Métodos que vamos mover do AuthService
        Task<AuthResult> RegisterAsync(UserDto request);
        Task<AuthResult> PromoteToAdminAsync(string userEmail);

        // O novo método que queremos adicionar (Tarefa 7)
        Task<AuthResult> CreateTenantUserAsync(CreateTenantUserDto request, Guid managerUserId, Guid tenantId);
    }
}