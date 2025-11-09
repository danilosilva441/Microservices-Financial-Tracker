// Caminho: backend/BillingService/Services/Interfaces/IUnidadeService.cs
using BillingService.DTO;
using BillingService.Models;

namespace BillingService.Services.Interfaces
{
    // 1. Renomeado
    public interface IUnidadeService
    {
        // 2. Atualizado para Unidade e DTOs de Unidade
        Task<IEnumerable<Unidade>> GetAllUnidadesAsync(Guid tenantId);
        // (Vamos precisar criar o UnidadeDto, por enquanto usamos OperacaoDto)
        Task<Unidade> CreateUnidadeAsync(OperacaoDto operacaoDto, Guid userId, Guid tenantId);
        Task<Unidade?> GetUnidadeByIdAsync(Guid id, Guid tenantId);
        Task<bool> UpdateUnidadeAsync(Guid id, UpdateOperacaoDto operacaoDto, Guid tenantId);
        Task<bool> DeactivateUnidadeAsync(Guid id, Guid tenantId);
        Task<bool> DeleteUnidadeAsync(Guid id, Guid tenantId);
        Task<bool> UpdateProjecaoAsync(Guid id, decimal projecao);
    }
}