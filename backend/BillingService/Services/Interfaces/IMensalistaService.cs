// Caminho: backend/BillingService/Services/Interfaces/IMensalistaService.cs
using BillingService.DTO;
using BillingService.Models;

namespace BillingService.Services.Interfaces
{
    // Métodos copiados do MensalistaService (v2.0)
    public interface IMensalistaService
    {
        Task<IEnumerable<Mensalista>> GetAllMensalistasAsync(
            Guid operacaoId, 
            Guid tenantId);
        Task<(Mensalista? mensalista, string? errorMessage)> CreateMensalistaAsync(
            Guid operacaoId, 
            CreateMensalistaDto mensalistaDto, 
            Guid tenantId);
        Task<(bool success, string? errorMessage)> UpdateMensalistaAsync(
            Guid operacaoId, 
            Guid mensalistaId, 
            UpdateMensalistaDto mensalistaDto, 
            Guid tenantId);
        Task<(bool success, string? errorMessage)> DeactivateMensalistaAsync(
            Guid operacaoId, 
            Guid mensalistaId, 
            Guid tenantId);
    }
}