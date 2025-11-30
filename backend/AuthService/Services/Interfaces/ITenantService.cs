// Caminho: backend/AuthService/Services/Interfaces/ITenantService.cs
using AuthService.DTO;

namespace AuthService.Services.Interfaces
{
    public interface ITenantService
    {
        Task<AuthResult> ProvisionTenantAsync(TenantProvisionDto request);
        Task<AuthResult> GetTenantByIdAsync(Guid tenantId);
        Task<AuthResult> UpdateTenantStatusAsync(Guid tenantId, string novoStatus);
        Task<AuthResult> GetTenantUsersAsync(Guid tenantId);
    }
}