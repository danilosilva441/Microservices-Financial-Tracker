// Caminho: backend/AuthService/Services/Interfaces/ITenantService.cs
using AuthService.DTO;

namespace AuthService.Services.Interfaces
{
    public interface ITenantService
    {
        // Método que vamos mover do AuthService
        Task<AuthResult> ProvisionTenantAsync(TenantProvisionDto request);
    }
}