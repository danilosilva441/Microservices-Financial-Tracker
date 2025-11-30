// Caminho: backend/BillingService/Controller/FaturamentoParcialController.cs
// (Antigo FaturamentosController.cs)
using BillingService.DTOs;
using BillingService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SharedKernel; // Importa o ErrorMessages

namespace BillingService.Controllers;

[ApiController]
// 1. MUDANÇA: Rota atualizada para v2.0
[Route("api/unidades/{unidadeId}/faturamentos-parciais")] 
[Authorize]
// 2. MUDANÇA: Nome da classe atualizado
public class FaturamentoParcialController : ControllerBase 
{
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
    
    private Guid GetTenantId()
    {
        var tenantIdClaim = User.FindFirst("tenantId")?.Value;
        if (tenantIdClaim == null) throw new InvalidOperationException("Tenant ID (tenantId) not found in token.");
        return Guid.Parse(tenantIdClaim);
    }

    [HttpPost]
    // 3. MUDANÇA: Parâmetro 'operacaoId' (v1.0) corrigido para 'unidadeId' (v2.0)
    public async Task<IActionResult> AddFaturamentoParcial(Guid unidadeId, [FromBody] FaturamentoParcialCreateDto faturamentoDto)
    {
        var userId = GetUserId();
        var tenantId = GetTenantId(); 

        var (novoFaturamento, errorMessage) = await _faturamentoService.AddFaturamentoAsync(unidadeId, faturamentoDto, userId, tenantId);

        if (errorMessage != null)
        {
            if (errorMessage == ErrorMessages.FechamentoJaExiste)
                return Conflict(errorMessage); // 409
            
            return NotFound(errorMessage); // 404
        }

        return Ok(novoFaturamento);
    }

    [HttpPut("{faturamentoId}")]
    [Authorize(Roles = "Admin, Gerente")]
    // 4. MUDANÇA: Parâmetro 'operacaoId' (v1.0) corrigido para 'unidadeId' (v2.0)
    public async Task<IActionResult> UpdateFaturamentoParcial(Guid unidadeId, Guid faturamentoId, [FromBody] FaturamentoParcialUpdateDto faturamentoDto)
    {
        var userId = GetUserId();
        var tenantId = GetTenantId(); 

        var (success, errorMessage) = await _faturamentoService.UpdateFaturamentoAsync(unidadeId, faturamentoId, faturamentoDto, userId, tenantId);

        if (!success)
        {
            if (errorMessage == ErrorMessages.OverlappingFaturamento)
                return Conflict(errorMessage); // 409
            
            return NotFound(errorMessage); // 404
        }

        return NoContent();
    }

    [HttpDelete("{faturamentoId}")]
    [Authorize(Roles = "Admin, Gerente")]
    // 5. MUDANÇA: Parâmetro 'operacaoId' (v1.0) corrigido para 'unidadeId' (v2.0)
    public async Task<IActionResult> DeleteFaturamentoParcial(Guid unidadeId, Guid faturamentoId)
    {
        var userId = GetUserId();
        var tenantId = GetTenantId(); 

        var (success, errorMessage) = await _faturamentoService.DeleteFaturamentoAsync(unidadeId, faturamentoId, userId, tenantId);

        if (!success)
        {
            return NotFound(errorMessage);
        }

        return NoContent();
    }

    [HttpPatch("{faturamentoId}/desativar")]
    [Authorize(Roles = "Admin, Gerente, Supervisor")] // Protegido
    // 6. MUDANÇA: Parâmetro 'operacaoId' (v1.0) corrigido para 'unidadeId' (v2.0)
    public async Task<IActionResult> DeactivateFaturamentoParcial(Guid unidadeId, Guid faturamentoId)
    {
        var userId = GetUserId();
        var tenantId = GetTenantId(); 

        var (success, errorMessage) = await _faturamentoService.DeactivateFaturamentoAsync(unidadeId, faturamentoId, userId, tenantId);

        if (!success)
        {
            return NotFound(errorMessage);
        }

        return NoContent();
    }
}