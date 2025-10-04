using BillingService.DTO;
using BillingService.Services;
using BillingService.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BillingService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Todos os endpoints aqui exigem login, a menos que especificado o contrário
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

    [HttpGet]
    [Authorize] // Agora está protegido por JWT padrão, sem políticas complexas
    public async Task<IActionResult> GetOperacoes([FromQuery] int? ano, [FromQuery] int? mes, [FromQuery] bool? isAtiva)
    {
        // Pega o email do token JWT do chamador
        var callerEmail = User.FindFirstValue(ClaimTypes.Email);

        // Se a chamada vier do nosso usuário de sistema...
        if (callerEmail == "system@internal.service")
        {
            // ...retorna TODAS as operações, pois é um serviço interno confiável.
            var todasOperacoes = await _operacaoService.GetAllOperacoesAsync(ano, mes, isAtiva);
            return Ok(todasOperacoes);
        }
        else
        {
            // Senão, é um usuário comum, então aplica a lógica de segurança padrão.
            var userId = GetUserId();
            var operacoes = await _operacaoService.GetOperacoesByUserAsync(userId, ano, mes, isAtiva);
            return Ok(operacoes);
        }
    }

    // --- NENHUMA ALTERAÇÃO NECESSÁRIA NOS MÉTODOS ABAIXO ---

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

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateOperacao([FromBody] OperacaoDto operacaoDto)
    {
        var userId = GetUserId();
        var novaOperacao = await _operacaoService.CreateOperacaoAsync(operacaoDto, userId);
        return CreatedAtAction(nameof(GetOperacaoById), new { id = novaOperacao.Id }, novaOperacao);
    }

    // PUT /api/operacoes/{id}
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateOperacao(Guid id, [FromBody] UpdateOperacaoDto operacaoDto)
    {
        var userId = GetUserId();
        // O método do serviço retorna um booleano simples.
        var success = await _operacaoService.UpdateOperacaoAsync(id, operacaoDto, userId);

        if (!success)
        {
            // Retorna um erro genérico, pois o serviço já logou o detalhe.
            return NotFound("Operação não encontrada ou usuário sem permissão.");
        }
        return NoContent();
    }

    // PATCH /api/operacoes/{id}/desativar
    [HttpPatch("{id}/desativar")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeactivateOperacao(Guid id)
    {
        var userId = GetUserId();
        var success = await _operacaoService.DeactivateOperacaoAsync(id, userId);
        if (!success)
        {
            return NotFound("Operação não encontrada ou usuário sem permissão.");
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

    [HttpPatch("{id}/projecao")]
    [Authorize]
    public async Task<IActionResult> UpdateProjecaoFaturamento(Guid id, [FromBody] ProjecaoDto projecaoDto)
    {
        var callerEmail = User.FindFirstValue(ClaimTypes.Email);
        // Apenas o serviço interno pode chamar este endpoint
        if (callerEmail != "system@internal.service")
        {
            return Forbid();
        }

        var success = await _operacaoService.UpdateProjecaoAsync(id, projecaoDto.ProjecaoFaturamento);
        if (!success)
        {
            return NotFound("Operação não encontrada.");
        }
        return NoContent();
    }
}