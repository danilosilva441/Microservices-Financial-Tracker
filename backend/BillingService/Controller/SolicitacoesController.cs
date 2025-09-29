using BillingService.DTOs;
using BillingService.Models;
using BillingService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BillingService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SolicitacoesController : ControllerBase
{
    private readonly SolicitacaoService _service;

    public SolicitacoesController(SolicitacaoService service)
    {
        _service = service;
    }

    private Guid GetUserId()
    {
        return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }

    [HttpPost]
    public async Task<IActionResult> CriarSolicitacao([FromBody] CriarSolicitacaoDto dto)
    {
        var userId = GetUserId();
        // Mapeia manualmente do DTO para a entidade do banco de dados
        var novaSolicitacao = new SolicitacaoAjuste
        {
            FaturamentoId = dto.FaturamentoId,
            Tipo = dto.Tipo,
            Motivo = dto.Motivo,
            DadosAntigos = dto.DadosAntigos,
            DadosNovos = dto.DadosNovos,
            Status = "PENDENTE"
        };
        var solicitacaoCriada = await _service.CriarSolicitacaoAsync(novaSolicitacao, userId);
        return Ok(solicitacaoCriada);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetSolicitacoes()
    {
        var solicitacoes = await _service.GetSolicitacoesAsync();
        return Ok(solicitacoes);
    }

    public class RevisaoDto
    {
        public string Acao { get; set; } = string.Empty;
    }

    [HttpPatch("{id}/revisar")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RevisarSolicitacao(Guid id, [FromBody] RevisaoDto revisao)
    {
        var (success, errorMessage) = await _service.RevisarSolicitacaoAsync(id, revisao.Acao, GetUserId());
        if (!success)
        {
            return BadRequest(errorMessage);
        }
        return NoContent();
    }
}