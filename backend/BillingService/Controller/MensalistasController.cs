// Caminho: backend/BillingService/Controller/MensalistasController.cs
using BillingService.DTOs; // 1. IMPORTANTE: DTOs
using BillingService.Services.Interfaces; // 2. IMPORTANTE: Interfaces
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BillingService.Controllers;

[ApiController]
// 3. MUDANÇA: Rota v2.0
[Route("api/unidades/{unidadeId}/mensalistas")] 
[Authorize(Roles = "Admin, Gerente")] // Atualizado
public class MensalistasController : ControllerBase
{
    // 4. MUDANÇA: Injeta a INTERFACE v2.0
    private readonly IMensalistaService _mensalistaService;

    public MensalistasController(IMensalistaService mensalistaService)
    {
        _mensalistaService = mensalistaService;
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null) throw new InvalidOperationException("User ID not found in token.");
        return Guid.Parse(userIdClaim);
    }

    // 5. NOVO: Helper v2.0
    private Guid GetTenantId()
    {
        var tenantIdClaim = User.FindFirst("tenantId")?.Value;
        if (tenantIdClaim == null) throw new InvalidOperationException("Tenant ID (tenantId) not found in token.");
        return Guid.Parse(tenantIdClaim);
    }

    [HttpGet]
    // 6. MUDANÇA: Assinatura v2.0
    public async Task<IActionResult> GetAll(Guid unidadeId)
    {
        var tenantId = GetTenantId();
        var mensalistas = await _mensalistaService.GetAllMensalistasAsync(unidadeId, tenantId);
        return Ok(mensalistas);
    }

    [HttpPost]
    // 7. MUDANÇA: Assinatura v2.0
    public async Task<IActionResult> Create(Guid unidadeId, [FromBody] CreateMensalistaDto mensalistaDto)
    {
        var tenantId = GetTenantId();
        var (novoMensalista, errorMessage) = await _mensalistaService.CreateMensalistaAsync(unidadeId, mensalistaDto, tenantId);

        if (errorMessage != null)
        {
            return NotFound(errorMessage);
        }

        return Ok(novoMensalista);
    }

    [HttpPut("{mensalistaId}")]
    // 8. MUDANÇA: Assinatura v2.0
    public async Task<IActionResult> Update(Guid unidadeId, Guid mensalistaId, [FromBody] UpdateMensalistaDto mensalistaDto)
    {
        var tenantId = GetTenantId();
        var (success, errorMessage) = await _mensalistaService.UpdateMensalistaAsync(unidadeId, mensalistaId, mensalistaDto, tenantId);

        if (!success)
        {
            return NotFound(errorMessage);
        }

        return NoContent();
    }

    [HttpPatch("{mensalistaId}/desativar")]
    // 9. MUDANÇA: Assinatura v2.0
    public async Task<IActionResult> Deactivate(Guid unidadeId, Guid mensalistaId)
    {
        var tenantId = GetTenantId();
        var (success, errorMessage) = await _mensalistaService.DeactivateMensalistaAsync(unidadeId, mensalistaId, tenantId);

        if (!success)
        {
            return NotFound(errorMessage);
        }

        return NoContent();
    }
}