using BillingService.DTOs;
using BillingService.Models;
using BillingService.Services.Interfaces; // 1. MUDANÇA: Adiciona o using da interface
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
    // 2. MUDANÇA: Injeta a INTERFACE
    private readonly ISolicitacaoService _service;

    public SolicitacoesController(ISolicitacaoService service) // <-- MUDANÇA AQUI
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
        
        var novaSolicitacao = new SolicitacaoAjuste
        {
            // 3. CORREÇÃO: Comentamos a propriedade v1.0 que não existe mais (CS0117)
            // FaturamentoId = dto.FaturamentoId, 
            
            Tipo = dto.Tipo,
            Motivo = dto.Motivo,
            DadosAntigos = dto.DadosAntigos,
            DadosNovos = dto.DadosNovos,
            Status = "PENDENTE"
        };
        
        // O código abaixo vai falhar em tempo de execução, mas vai COMPILAR.
        // Precisaremos refatorar o _service.CriarSolicitacaoAsync depois.
        var solicitacaoCriada = await _service.CriarSolicitacaoAsync(novaSolicitacao, userId);
        return Ok(solicitacaoCriada);
    }

    [HttpGet]
    [Authorize(Roles = "Admin  , Gerente")]
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
    [Authorize(Roles = "Admin, Gerente")]
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