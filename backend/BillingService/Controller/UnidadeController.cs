// Caminho: backend/BillingService/Controller/UnidadesController.cs
using BillingService.DTO;
using BillingService.Services.Interfaces; // 1. MUDANÇA: Usando a interface v2.0
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BillingService.Controllers;

[ApiController]
[Route("api/unidades")] // 2. MUDANÇA: Rota atualizada
[Authorize]
public class UnidadesController : ControllerBase // 3. MUDANÇA: Nome da classe
{
    // 4. MUDANÇA: Injetando a interface v2.0
    private readonly IUnidadeService _unidadeService;

    public UnidadesController(IUnidadeService unidadeService)
    {
        _unidadeService = unidadeService;
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

    [HttpGet]
    public async Task<IActionResult> GetUnidades()
    {
        // 5. MUDANÇA: Lógica v2.0
        var tenantId = GetTenantId();
        var unidades = await _unidadeService.GetAllUnidadesAsync(tenantId);
        return Ok(unidades);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUnidadeById(Guid id)
    {
        var tenantId = GetTenantId();
        var unidade = await _unidadeService.GetUnidadeByIdAsync(id, tenantId);
        if (unidade == null)
        {
            return NotFound();
        }
        return Ok(unidade);
    }

    [HttpPost]
    [Authorize(Roles = "Admin, Gerente")]
    public async Task<IActionResult> CreateUnidade([FromBody] OperacaoDto operacaoDto) // (Ainda usa o DTO antigo, vamos corrigir depois)
    {
        var userId = GetUserId();
        var tenantId = GetTenantId();
        var novaUnidade = await _unidadeService.CreateUnidadeAsync(operacaoDto, userId, tenantId);
        return CreatedAtAction(nameof(GetUnidadeById), new { id = novaUnidade.Id }, novaUnidade);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin, Gerente")]
    public async Task<IActionResult> UpdateUnidade(Guid id, [FromBody] UpdateOperacaoDto operacaoDto)
    {
        var tenantId = GetTenantId();
        var success = await _unidadeService.UpdateUnidadeAsync(id, operacaoDto, tenantId);

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
        var success = await _unidadeService.DeleteUnidadeAsync(id, tenantId);
        if (!success)
        {
            return NotFound("Unidade não encontrada ou não pertence ao usuário.");
        }
        return NoContent();
    }

    [HttpPatch("{id}/projecao")]
    [Authorize(Roles = "system@internal.service")] // Protegendo pelo "Role" do serviço
    public async Task<IActionResult> UpdateProjecaoFaturamento(Guid id, [FromBody] ProjecaoDto projecaoDto)
    {
        var success = await _unidadeService.UpdateProjecaoAsync(id, projecaoDto.ProjecaoFaturamento);
        if (!success)
        {
            return NotFound("Unidade não encontrada.");
        }
        return NoContent();
    }
}