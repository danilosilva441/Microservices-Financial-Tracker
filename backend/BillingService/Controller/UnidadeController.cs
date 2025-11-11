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
        // Se o TenantId for nulo (ex: Admin/Sistema), lança exceção
        if (tenantIdClaim == null) 
            throw new InvalidOperationException("Tenant ID (tenantId) not found in token. Este endpoint é para Gerentes.");
        return Guid.Parse(tenantIdClaim);
    }

    // Helper v2.0 para checar se é Admin/Sistema (TenantId NULO)
    private bool IsAdmin()
    {
        return User.FindFirst("tenantId") == null;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetUnidades()
    {
        // 7. MUDANÇA: Lógica v2.0 (Admin vs Gerente)
        // Esta é a lógica que corrige o bug do "Nenhum dado encontrado"
        if (IsAdmin())
        {
            var todasUnidades = await _unidadeService.GetAllUnidadesAdminAsync();
            return Ok(todasUnidades);
        }
        else
        {
            var tenantId = GetTenantId();
            var unidades = await _unidadeService.GetAllUnidadesByTenantAsync(tenantId);
            return Ok(unidades);
        }
    }

    [HttpGet("{id}")]
    [Authorize]
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
    // 8. MUDANÇA: Usa o DTO v2.0
    public async Task<IActionResult> CreateUnidade([FromBody] UnidadeDto unidadeDto)
    {
        var userId = GetUserId();
        var tenantId = GetTenantId();
        
        var novaUnidade = await _unidadeService.CreateUnidadeAsync(unidadeDto, userId, tenantId);
        return CreatedAtAction(nameof(GetUnidadeById), new { id = novaUnidade.Id }, novaUnidade);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin, Gerente")]
    // 9. MUDANÇA: Usa o DTO v2.0
    public async Task<IActionResult> UpdateUnidade(Guid id, [FromBody] UpdateUnidadeDto unidadeDto)
    {
        var tenantId = GetTenantId();
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
    [Authorize(Roles = "Admin")] // Protegido para Admin/Sistema
    public async Task<IActionResult> UpdateProjecaoFaturamento(Guid id, [FromBody] ProjecaoDto projecaoDto)
    {
        var success = await _unidadeService.UpdateProjecaoAsync(id, projecaoDto.ProjecaoFaturamento);
        if (!success)
        {
            return NotFound("Operação não encontrada.");
        }
        return NoContent();
    }
}