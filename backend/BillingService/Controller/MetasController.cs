using BillingService.DTO;
using BillingService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BillingService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MetasController : ControllerBase
{
    private readonly MetaService _metaService;

    public MetasController(MetaService metaService)
    {
        _metaService = metaService;
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null) throw new InvalidOperationException("User ID not found in token.");
        return Guid.Parse(userIdClaim);
    }

    [HttpGet]
    public async Task<IActionResult> GetMeta([FromQuery] int mes, [FromQuery] int ano)
    {
        var userId = GetUserId();
        var meta = await _metaService.GetMetaAsync(userId, mes, ano);

        if (meta == null)
        {
            return NotFound("Meta para este período não encontrada.");
        }

        return Ok(meta);
    }

    [HttpPost]
    [Authorize(Roles = "Admin , Gerente")]
    public async Task<IActionResult> SetMeta([FromBody] MetaDto metaDto)
    {
        var userId = GetUserId();
        var metaSalva = await _metaService.SetMetaAsync(metaDto, userId);
        return Ok(metaSalva);
    }
}