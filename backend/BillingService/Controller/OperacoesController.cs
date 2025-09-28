using BillingService.DTO;
using BillingService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BillingService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Todos precisam estar logados para acessar qualquer endpoint aqui
public class OperacoesController : ControllerBase
{
    private readonly OperacaoService _operacaoService;

    public OperacoesController(OperacaoService operacaoService)
    {
        _operacaoService = operacaoService;
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null) throw new InvalidOperationException("User ID not found in token.");
        return Guid.Parse(userIdClaim);
    }

    // GETs podem ser acessados por qualquer usuário autenticado (a lógica de vínculo já protege os dados)
    // GET /api/operacoes
[HttpGet]
public async Task<IActionResult> GetOperacoes([FromQuery] int? ano, [FromQuery] int? mes, [FromQuery] bool? isAtiva)
{
    // --- VERIFICAÇÃO PARA SERVIÇO INTERNO ---
    var remoteIpAddress = HttpContext.Connection.RemoteIpAddress;
    // Se a requisição vem de um IP da rede interna do Docker, não precisa de UserId.
    // Isso permite que o AnalysisService busque todos os dados.
    if (remoteIpAddress != null && remoteIpAddress.ToString().StartsWith("172.")) // IPs do Docker geralmente começam com 172.
    {
        // Busca TODAS as operações, sem filtrar por usuário
        var todasOperacoes = await _operacaoService.GetAllOperacoesAsync(ano, mes, isAtiva);
        return Ok(todasOperacoes);
    }
    // ------------------------------------------

    // Se não for uma chamada interna, a lógica de permissão por usuário continua
    var userId = GetUserId();
    var operacoes = await _operacaoService.GetOperacoesByUserAsync(userId, ano, mes, isAtiva);
    return Ok(operacoes);
}

    // GET /api/operacoes/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOperacaoById(Guid id)
    {
        var userId = GetUserId();
        var operacao = await _operacaoService.GetOperacaoByIdAsync(id, userId);
        if (operacao == null)
        {
            return NotFound();
        }
        return Ok(operacao);
    }

    // POST /api/operacoes
    [HttpPost]
    [Authorize(Roles = "Admin")] // Apenas Admins podem criar novas operações
    public async Task<IActionResult> CreateOperacao([FromBody] OperacaoDto operacaoDto)
    {
        var userId = GetUserId();
        var novaOperacao = await _operacaoService.CreateOperacaoAsync(operacaoDto, userId);
        return CreatedAtAction(nameof(GetOperacaoById), new { id = novaOperacao.Id }, novaOperacao);
    }

    // PUT /api/operacoes/{id}
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")] // Apenas Admins podem atualizar operações
    public async Task<IActionResult> UpdateOperacao(Guid id, [FromBody] UpdateOperacaoDto operacaoDto)
    {
        var userId = GetUserId();
        var success = await _operacaoService.UpdateOperacaoAsync(id, operacaoDto, userId);

        if (!success)
        {
            return NotFound("Operação não encontrada ou não pertence ao usuário.");
        }
        return NoContent();
    }

    // PATCH /api/operacoes/{id}/desativar
    [HttpPatch("{id}/desativar")]
    [Authorize(Roles = "Admin")] // Apenas Admins podem desativar operações
    public async Task<IActionResult> DeactivateOperacao(Guid id)
    {
        var userId = GetUserId();
        var success = await _operacaoService.DeactivateOperacaoAsync(id, userId);
        if (!success)
        {
            return NotFound("Operação não encontrada ou não pertence ao usuário.");
        }
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteOperacao(Guid id)
    {
        var success = await _operacaoService.DeleteOperacaoAsync(id, GetUserId());
        if (!success)
        {
            return NotFound("Operação não encontrada ou não pertence ao usuário.");
        }
        return NoContent();
    }
}