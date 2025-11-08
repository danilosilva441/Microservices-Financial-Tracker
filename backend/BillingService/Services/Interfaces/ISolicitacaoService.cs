// Caminho: backend/BillingService/Services/Interfaces/ISolicitacaoService.cs
using BillingService.DTOs;
using BillingService.Models;

namespace BillingService.Services.Interfaces
{
    public interface ISolicitacaoService
    {
        // 1. Assinatura para CriarSolicitacaoAsync
        Task<SolicitacaoAjuste> CriarSolicitacaoAsync(SolicitacaoAjuste solicitacao, Guid solicitanteId);

        // 2. Assinatura para GetSolicitacoesAsync (retorna o DTO v2.0)
        Task<IEnumerable<SolicitacaoAjusteDto>> GetSolicitacoesAsync();

        // 3. Assinatura para RevisarSolicitacaoAsync
        Task<(bool success, string? errorMessage)> RevisarSolicitacaoAsync(Guid id, string acao, Guid aprovadorId);
    }
}