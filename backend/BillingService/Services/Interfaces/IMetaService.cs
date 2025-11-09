// Caminho: backend/BillingService/Services/Interfaces/IMetaService.cs
using BillingService.DTOs; // Vamos precisar do MetaDto
using BillingService.Models;

namespace BillingService.Services.Interfaces
{
    public interface IMetaService
    {
        // Métodos v2.0
        Task<Meta?> GetMetaAsync(Guid unidadeId, int mes, int ano, Guid tenantId);
        Task<IEnumerable<Meta>> GetMetasAsync(Guid unidadeId, Guid tenantId);
        Task<(Meta? meta, string? errorMessage)> SetMetaAsync(Guid unidadeId, MetaDto metaDto, Guid tenantId);
    }
}