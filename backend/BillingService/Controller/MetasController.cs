// Caminho: backend/BillingService/Controller/MetasController.cs
using BillingService.DTOs;
using BillingService.Services.Interfaces; // 1. IMPORTANTE: Usando Interfaces
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BillingService.Controllers;

[ApiController]
// 2. MUDANÇA (v2.0): A rota agora está "aninhada" dentro da Unidade
[Route("api/unidades/{unidadeId}/metas")] 
[Authorize]
public class MetasController : ControllerBase
{
    // 3. MUDANÇA: Injeta a INTERFACE v2.0
    private readonly IMetaService _metaService;

    public MetasController(IMetaService metaService)
    {
        _metaService = metaService;
    }

    // 4. NOVO (v2.0): Helper para pegar o TenantId
    private Guid GetTenantId()
    {
        var tenantIdClaim = User.FindFirst("tenantId")?.Value;
        if (tenantIdClaim == null) throw new InvalidOperationException("Tenant ID (tenantId) not found in token.");
        return Guid.Parse(tenantIdClaim);
    }

    // (O GetUserId() foi removido pois a lógica v2.0 não o utiliza aqui)

    /// <summary>
    /// Lista todas as metas de uma unidade específica.
    /// </summary>
    [HttpGet]
    // 5. MUDANÇA: Assinatura v2.0
    public async Task<IActionResult> GetMetas(Guid unidadeId)
    {
        var tenantId = GetTenantId();
        var metas = await _metaService.GetMetasAsync(unidadeId, tenantId);
        return Ok(metas);
    }

    /// <summary>
    /// Busca uma meta específica pelo período (mês/ano).
    /// </summary>
    [HttpGet("periodo")] // Rota: /api/unidades/{unidadeId}/metas/periodo?mes=11&ano=2025
    // 6. MUDANÇA: Assinatura v2.0
    public async Task<IActionResult> GetMetaPorPeriodo(Guid unidadeId, [FromQuery] int mes, [FromQuery] int ano)
    {
        var tenantId = GetTenantId();
        var meta = await _metaService.GetMetaAsync(unidadeId, mes, ano, tenantId);

        if (meta == null)
        {
            return NotFound("Meta para este período não encontrada.");
        }

        return Ok(meta);
    }

    /// <summary>
    /// Cria ou atualiza a meta para um período.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin, Gerente")] // Roles v2.0
    // 7. MUDANÇA: Assinatura v2.0
    public async Task<IActionResult> SetMeta(Guid unidadeId, [FromBody] MetaDto metaDto)
    {
        var tenantId = GetTenantId();
        var (metaSalva, errorMessage) = await _metaService.SetMetaAsync(unidadeId, metaDto, tenantId);
        
        if (errorMessage != null)
        {
            if (errorMessage.Contains("não encontrada"))
                return NotFound(errorMessage); // Erro 404
            
            return BadRequest(errorMessage); // Erro 400
        }
        
        return Ok(metaSalva);
    }
}