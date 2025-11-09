using AuthService.DTO;

namespace AuthService.Services
{
    // Define o contrato para a lógica de autenticação e gestão de usuários
    public interface IAuthService
    {
        Task<AuthResult> RegisterAsync(UserDto request);
        Task<AuthResult> LoginAsync(UserDto request);
        Task<AuthResult> PromoteToAdminAsync(string userEmail);
        
        // --- NOVA FUNÇÃO (Fase 1 / Tarefa 2.4) ---
        Task<AuthResult> ProvisionTenantAsync(TenantProvisionDto request);
    }
}