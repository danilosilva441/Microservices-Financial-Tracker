using BillingService.DTO;
using BillingService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BillingService.Controllers;

[ApiController]
[Route("api/operacoes/{operacaoId}/faturamentos")]
[Authorize]
public class FaturamentosController : ControllerBase
{
    private readonly FaturamentoService _faturamentoService;

    public FaturamentosController(FaturamentoService faturamentoService)
    {
        _faturamentoService = faturamentoService;
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null) throw new InvalidOperationException("User ID not found in token.");
        return Guid.Parse(userIdClaim);
    }

    [HttpPost]
    public async Task<IActionResult> AddFaturamento(Guid operacaoId, [FromBody] FaturamentoDto faturamentoDto)
    {
        var userId = GetUserId();
        var (novoFaturamento, errorMessage) = await _faturamentoService.AddFaturamentoAsync(operacaoId, faturamentoDto, userId);

        if (errorMessage != null)
        {
            return errorMessage.Contains("Já existe") ? Conflict(errorMessage) : NotFound(errorMessage);
        }

        return Ok(novoFaturamento);
    }

    [HttpPut("{faturamentoId}")]
    public async Task<IActionResult> UpdateFaturamento(Guid operacaoId, Guid faturamentoId, [FromBody] UpdateFaturamentoDto faturamentoDto)
    {
        var userId = GetUserId();
        var (success, errorMessage) = await _faturamentoService.UpdateFaturamentoAsync(operacaoId, faturamentoId, faturamentoDto, userId);

        if (!success)
        {
            return errorMessage!.Contains("Já existe") ? Conflict(errorMessage) : NotFound(errorMessage);
        }

        return NoContent();
    }

    [HttpDelete("{faturamentoId}")]
    public async Task<IActionResult> DeleteFaturamento(Guid operacaoId, Guid faturamentoId)
    {
        var userId = GetUserId();
        var (success, errorMessage) = await _faturamentoService.DeleteFaturamentoAsync(operacaoId, faturamentoId, userId);

        if (!success)
        {
            return NotFound(errorMessage);
        }

        return NoContent();
    }
}