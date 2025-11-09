using BillingService.DTO;
using BillingService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BillingService.Controllers;

[ApiController]
[Route("api/operacoes/{operacaoId}/faturamentos")]
[Authorize]
public class FaturamentosController : ControllerBase
{
    // 1. MUDANÇA: Injetar a INTERFACE, não a classe concreta
    // Isso é crucial para a Injeção de Dependência funcionar
    private readonly IFaturamentoParcialService _faturamentoService;

    public FaturamentosController(IFaturamentoParcialService faturamentoService) // <-- MUDANÇA AQUI
    {
        _faturamentoService = faturamentoService;
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null) throw new InvalidOperationException("User ID not found in token.");
        return Guid.Parse(userIdClaim);
    }

    // 2. NOVA FUNÇÃO: Pegar o TenantId que o AuthService nos deu
    private Guid GetTenantId()
    {
        var tenantIdClaim = User.FindFirst("tenantId")?.Value;
        if (tenantIdClaim == null) throw new InvalidOperationException("Tenant ID (tenantId) not found in token.");
        return Guid.Parse(tenantIdClaim);
    }

    [HttpPost]
    public async Task<IActionResult> AddFaturamento(Guid operacaoId, [FromBody] FaturamentoParcialCreateDto faturamentoDto)
    {
        var userId = GetUserId();
        var tenantId = GetTenantId(); // <-- PEGANDO O TENANT ID

        // 3. MUDANÇA: Passando o tenantId para o serviço
        var (novoFaturamento, errorMessage) = await _faturamentoService.AddFaturamentoAsync(operacaoId, faturamentoDto, userId, tenantId);

        if (errorMessage != null)
        {
            return errorMessage.Contains("Já existe") ? Conflict(errorMessage) : NotFound(errorMessage);
        }

        return Ok(novoFaturamento);
    }

    [HttpPut("{faturamentoId}")]
    [Authorize(Roles = "Admin, Gerente")]
    public async Task<IActionResult> UpdateFaturamento(Guid operacaoId, Guid faturamentoId, [FromBody] FaturamentoParcialUpdateDto faturamentoDto)
    {
        var userId = GetUserId();
        var tenantId = GetTenantId(); // <-- PEGANDO O TENANT ID

        // 4. MUDANÇA: Passando o tenantId para o serviço
        var (success, errorMessage) = await _faturamentoService.UpdateFaturamentoAsync(operacaoId, faturamentoId, faturamentoDto, userId, tenantId);

        if (!success)
        {
            return errorMessage!.Contains("Já existe") ? Conflict(errorMessage) : NotFound(errorMessage);
        }

        return NoContent();
    }

    [HttpDelete("{faturamentoId}")]
    [Authorize(Roles = "Admin, Gerente")]
    public async Task<IActionResult> DeleteFaturamento(Guid operacaoId, Guid faturamentoId)
    {
        var userId = GetUserId();
        var tenantId = GetTenantId(); // <-- PEGANDO O TENANT ID

        // 5. MUDANÇA: Passando o tenantId para o serviço
        var (success, errorMessage) = await _faturamentoService.DeleteFaturamentoAsync(operacaoId, faturamentoId, userId, tenantId);

        if (!success)
        {
            return NotFound(errorMessage);
        }

        return NoContent();
    }

    [HttpPatch("{faturamentoId}/desativar")]
    public async Task<IActionResult> DeactivateFaturamento(Guid operacaoId, Guid faturamentoId)
    {
        var userId = GetUserId();
        var tenantId = GetTenantId(); // <-- PEGANDO O TENANT ID

        // 6. MUDANÇA: Passando o tenantId para o serviço
        var (success, errorMessage) = await _faturamentoService.DeactivateFaturamentoAsync(operacaoId, faturamentoId, userId, tenantId);

        if (!success)
        {
            return NotFound(errorMessage);
        }

        return NoContent();
    }
}