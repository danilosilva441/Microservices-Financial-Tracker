using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BillingService.Data;
using BillingService.DTO;
using BillingService.Models;

namespace BillingService.Controllers;

[ApiController]
[Route("api/operacoes/{operacaoId}/[controller]")] // Rota será /api/operacoes/{id}/faturamentos
[Authorize]
public class FaturamentosController : ControllerBase
{
    private readonly BillingDbContext _context;

    public FaturamentosController(BillingDbContext context)
    {
        _context = context;
    }

    private Guid GetUserId()
    {
        // ... (código do GetUserId é o mesmo)
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null) throw new InvalidOperationException("User ID not found in token.");
        return Guid.Parse(userIdClaim);
    }

    [HttpPost] // POST /api/operacoes/{operacaoId}/faturamentos
    public async Task<IActionResult> AddFaturamento(Guid operacaoId, [FromBody] FaturamentoDto faturamentoDto)
    {
        var userId = GetUserId();
        var operacao = await _context.Operacoes
            .FirstOrDefaultAsync(op => op.Id == operacaoId && op.UserId == userId);

        if (operacao == null)
        {
            return NotFound("A operação especificada não foi encontrada ou não pertence a este usuário.");
        }

        var novoFaturamento = new Faturamento
        {
            Id = Guid.NewGuid(),
            Valor = faturamentoDto.Valor,
            Data = faturamentoDto.Data.ToUniversalTime(),
            Moeda = faturamentoDto.Moeda, // <-- Adicione esta linha
            OperacaoId = operacaoId
        };

        await _context.Faturamentos.AddAsync(novoFaturamento);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(AddFaturamento), new { id = novoFaturamento.Id }, novoFaturamento);
    }
}