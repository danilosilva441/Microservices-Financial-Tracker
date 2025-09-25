using BillingService.DTO;
using BillingService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BillingService.Controllers;

[ApiController]
[Route("api/operacoes/{operacaoId}/mensalistas")]
[Authorize(Roles = "Admin")]
public class MensalistasController : ControllerBase
{
    private readonly MensalistaService _mensalistaService;

    public MensalistasController(MensalistaService mensalistaService)
    {
        _mensalistaService = mensalistaService;
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null) throw new InvalidOperationException("User ID not found in token.");
        return Guid.Parse(userIdClaim);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(Guid operacaoId)
    {
        var userId = GetUserId();
        var mensalistas = await _mensalistaService.GetAllMensalistasAsync(operacaoId, userId);
        return Ok(mensalistas);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Guid operacaoId, [FromBody] CreateMensalistaDto mensalistaDto)
    {
        var userId = GetUserId();
        var (novoMensalista, errorMessage) = await _mensalistaService.CreateMensalistaAsync(operacaoId, mensalistaDto, userId);

        if (errorMessage != null)
        {
            return NotFound(errorMessage);
        }

        return Ok(novoMensalista);
    }

    [HttpPut("{mensalistaId}")]
    public async Task<IActionResult> Update(Guid operacaoId, Guid mensalistaId, [FromBody] UpdateMensalistaDto mensalistaDto)
    {
        var userId = GetUserId();
        var (success, errorMessage) = await _mensalistaService.UpdateMensalistaAsync(operacaoId, mensalistaId, mensalistaDto, userId);

        if (!success)
        {
            return NotFound(errorMessage);
        }

        return NoContent();
    }

    [HttpPatch("{mensalistaId}/desativar")]
    public async Task<IActionResult> Deactivate(Guid operacaoId, Guid mensalistaId)
    {
        var userId = GetUserId();
        var (success, errorMessage) = await _mensalistaService.DeactivateMensalistaAsync(operacaoId, mensalistaId, userId);

        if (!success)
        {
            return NotFound(errorMessage);
        }

        return NoContent();
    }
}