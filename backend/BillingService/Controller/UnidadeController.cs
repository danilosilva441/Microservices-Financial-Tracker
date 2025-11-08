// Caminho: backend/BillingService/Controller/UnidadeController.cs
using BillingService.DTOs; // 1. IMPORTANTE: Usando DTOs v2.0
using BillingService.Services.Interfaces; // 2. IMPORTANTE: Usando Interfaces v2.0
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BillingService.Controllers;

[ApiController]
[Route("api/unidades")] // 3. MUDANÇA: Rota v2.0
[Authorize] 
public class UnidadeController : ControllerBase // 4. MUDANÇA: Nome da classe v2.0
{
    // 5. MUDANÇA: Injeta a INTERFACE v2.0
    private readonly IUnidadeService _unidadeService;

    public UnidadeController(IUnidadeService unidadeService)
    {
        _unidadeService = unidadeService;
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

    [HttpGet]
    public async Task<IActionResult> GetUnidades()
    {
        // 7. MUDANÇA: Lógica v2.0
        // (Removemos a lógica complexa de "system@internal.service" da v1.0)
        var tenantId = GetTenantId();
        var unidades = await _unidadeService.GetAllUnidadesAsync(tenantId);
        return Ok(unidades);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUnidadeById(Guid id)
    {
        var tenantId = GetTenantId();
        // 8. MUDANÇA: Chama o método v2.0
        var unidade = await _unidadeService.GetUnidadeByIdAsync(id, tenantId);
        if (unidade == null)
        {
            return NotFound();
        }
        return Ok(unidade);
    }

    [HttpPost]
    [Authorize(Roles = "Admin, Gerente")] // (v2.0 Roles)
    // 9. MUDANÇA: Usa o DTO v2.0
    public async Task<IActionResult> CreateUnidade([FromBody] UnidadeDto unidadeDto)
    {
        var userId = GetUserId();
        var tenantId = GetTenantId();
        
        // 10. MUDANÇA: Chama o serviço v2.0
        var novaUnidade = await _unidadeService.CreateUnidadeAsync(unidadeDto, userId, tenantId);
        return CreatedAtAction(nameof(GetUnidadeById), new { id = novaUnidade.Id }, novaUnidade);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin, Gerente")]
    // 11. MUDANÇA: Usa o DTO v2.0
    public async Task<IActionResult> UpdateUnidade(Guid id, [FromBody] UpdateUnidadeDto unidadeDto)
    {
        var tenantId = GetTenantId();
        // 12. MUDANÇA: Chama o serviço v2.0
        var success = await _unidadeService.UpdateUnidadeAsync(id, unidadeDto, tenantId);

        if (!success)
        {
            return NotFound("Unidade não encontrada ou usuário sem permissão.");
        }
        return NoContent();
    }

    [HttpPatch("{id}/desativar")]
    [Authorize(Roles = "Admin, Gerente")]
    public async Task<IActionResult> DeactivateUnidade(Guid id)
    {
        var tenantId = GetTenantId();
        // 13. MUDANÇA: Chama o serviço v2.0
        var success = await _unidadeService.DeactivateUnidadeAsync(id, tenantId);
        if (!success)
        {
            return NotFound("Unidade não encontrada ou usuário sem permissão.");
        }
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin, Gerente")]
    public async Task<IActionResult> DeleteUnidade(Guid id)
    {
        var tenantId = GetTenantId();
        // 14. MUDANÇA: Chama o serviço v2.0
        var success = await _unidadeService.DeleteUnidadeAsync(id, tenantId);
        if (!success)
        {
            return NotFound("Unidade não encontrada ou não pertence ao usuário.");
        }
        return NoContent();
    }

    [HttpPatch("{id}/projecao")]
    [Authorize(Roles = "system@internal.service")] // Protegido por Role v2.0
    public async Task<IActionResult> UpdateProjecaoFaturamento(Guid id, [FromBody] ProjecaoDto projecaoDto)
    {
        // Este método não precisa de tenantId pois é chamado por um serviço interno
        var success = await _unidadeService.UpdateProjecaoAsync(id, projecaoDto.ProjecaoFaturamento);
        if (!success)
        {
            return NotFound("Operação não encontrada.");
        }
        return NoContent();
    }
}