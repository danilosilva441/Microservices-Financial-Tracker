// Caminho: backend/BillingService/Services/Interfaces/IMensalistaService.cs
using BillingService.DTOs; // Precisaremos dos DTOs
using BillingService.Models;

namespace BillingService.Services.Interfaces
{
    public interface IMensalistaService
    {
        // 1. MUDANÇA: Assinaturas v2.0 (usam UnidadeId e TenantId)
        Task<IEnumerable<Mensalista>> GetAllMensalistasAsync(Guid unidadeId, Guid tenantId);
        Task<(Mensalista? mensalista, string? errorMessage)> CreateMensalistaAsync(Guid unidadeId, CreateMensalistaDto mensalistaDto, Guid tenantId);
        Task<(bool success, string? errorMessage)> UpdateMensalistaAsync(Guid unidadeId, Guid mensalistaId, UpdateMensalistaDto mensalistaDto, Guid tenantId);
        Task<(bool success, string? errorMessage)> DeactivateMensalistaAsync(Guid unidadeId, Guid mensalistaId, Guid tenantId);
    }
}