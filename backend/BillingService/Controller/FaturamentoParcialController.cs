// Caminho: backend/BillingService/Controller/FaturamentoParcialController.cs
// (Arquivo FaturamentosController.cs renomeado)
using BillingService.DTOs; // 1. IMPORTANTE: Usando DTOs
using BillingService.Services.Interfaces; // 2. IMPORTANTE: Usando Interfaces
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BillingService.Controllers;

[ApiController]
// 3. MUDANÇA: Rota v2.0 (agora usa "unidadeId")
[Route("api/unidades/{unidadeId}/faturamentos-parciais")] 
[Authorize]
public class FaturamentoParcialController : ControllerBase // 4. MUDANÇA: Nome da classe
{
    // 5. MUDANÇA: Injeta a INTERFACE v2.0
    private readonly IFaturamentoParcialService _faturamentoService;

    public FaturamentoParcialController(IFaturamentoParcialService faturamentoService)
    {
        _faturamentoService = faturamentoService;
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null) throw new InvalidOperationException("User ID not found in token.");
        return Guid.Parse(userIdClaim);
    }

    // 6. NOVO: Helper v2.0 para pegar o TenantId do token
    private Guid GetTenantId()
    {
        var tenantIdClaim = User.FindFirst("tenantId")?.Value;
        if (tenantIdClaim == null) throw new InvalidOperationException("Tenant ID (tenantId) not found in token.");
        return Guid.Parse(tenantIdClaim);
    }

    [HttpPost]
    // 7. MUDANÇA: Assinatura v2.0 (usa "unidadeId" da rota e DTO v2.0)
    public async Task<IActionResult> AddFaturamento(Guid unidadeId, [FromBody] FaturamentoParcialCreateDto faturamentoDto)
    {
        var userId = GetUserId();
        var tenantId = GetTenantId(); // Pega o TenantId

        // 8. MUDANÇA: Chama o serviço v2.0 com os parâmetros corretos
        var (novoFaturamento, errorMessage) = await _faturamentoService.AddFaturamentoAsync(unidadeId, faturamentoDto, userId, tenantId);

        if (errorMessage != null)
        {
            return errorMessage.Contains("Já existe") ? Conflict(errorMessage) : NotFound(errorMessage);
        }

        return Ok(novoFaturamento);
    }

    [HttpPut("{faturamentoId}")]
    [Authorize(Roles = "Admin, Gerente")]
    // 9. MUDANÇA: Assinatura v2.0
    public async Task<IActionResult> UpdateFaturamento(Guid unidadeId, Guid faturamentoId, [FromBody] FaturamentoParcialUpdateDto faturamentoDto)
    {
        var userId = GetUserId();
        var tenantId = GetTenantId();

        // 10. MUDANÇA: Chama o serviço v2.0
        var (success, errorMessage) = await _faturamentoService.UpdateFaturamentoAsync(unidadeId, faturamentoId, faturamentoDto, userId, tenantId);

        if (!success)
        {
            return errorMessage!.Contains("Já existe") ? Conflict(errorMessage) : NotFound(errorMessage);
        }

        return NoContent();
    }

    [HttpDelete("{faturamentoId}")]
    [Authorize(Roles = "Admin, Gerente")]
    public async Task<IActionResult> DeleteFaturamento(Guid unidadeId, Guid faturamentoId)
    {
        var userId = GetUserId();
        var tenantId = GetTenantId();

        // 11. MUDANÇA: Chama o serviço v2.0
        var (success, errorMessage) = await _faturamentoService.DeleteFaturamentoAsync(unidadeId, faturamentoId, userId, tenantId);

        if (!success)
        {
            return NotFound(errorMessage);
        }

        return NoContent();
    }

    [HttpPatch("{faturamentoId}/desativar")]
    [Authorize(Roles = "Admin, Gerente, Supervisor")] // Apenas usuários autorizados
    public async Task<IActionResult> DeactivateFaturamento(Guid unidadeId, Guid faturamentoId)
    {
        var userId = GetUserId();
        var tenantId = GetTenantId();

        // 12. MUDANÇA: Chama o serviço v2.0
        var (success, errorMessage) = await _faturamentoService.DeactivateFaturamentoAsync(unidadeId, faturamentoId, userId, tenantId);

        if (!success)
        {
            return NotFound(errorMessage);
        }

        return NoContent(); 
    }
}