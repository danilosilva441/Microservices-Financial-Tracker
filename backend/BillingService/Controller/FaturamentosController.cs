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
    [Authorize(Roles = "Admin")] // Apenas Admins podem atualizar faturamentos
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
    [Authorize(Roles = "Admin")] // Apenas Admins podem deletar faturamentos
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

    [HttpPatch("{faturamentoId}/desativar")]
    // Sem [Authorize(Roles = "Admin")], qualquer usuário vinculado pode desativar
    public async Task<IActionResult> DeactivateFaturamento(Guid operacaoId, Guid faturamentoId)
    {
        var userId = GetUserId();
        var (success, errorMessage) = await _faturamentoService.DeactivateFaturamentoAsync(operacaoId, faturamentoId, userId);

        if (!success)
        {
            return NotFound(errorMessage);
        }

        return NoContent(); // Retorna 204 No Content, indicando sucesso sem corpo de resposta
    }
}