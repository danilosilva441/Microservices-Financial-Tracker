using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BillingService.Data;
using BillingService.DTO;
using BillingService.Models;

namespace BillingService.Controllers;

[ApiController]
[Route("api/[controller]")] // Rota será /api/operacoes
[Authorize]
public class OperacoesController : ControllerBase
{
    private readonly BillingDbContext _context;

    public OperacoesController(BillingDbContext context)
    {
        _context = context;
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null)
        {
            throw new InvalidOperationException("User ID not found in token.");
        }
        return Guid.Parse(userIdClaim);
    }

    [HttpGet] // GET /api/operacoes?ano=2025&mes=9...
    public async Task<IActionResult> GetOperacoes([FromQuery] int? ano, [FromQuery] int? mes, [FromQuery] bool? isAtiva)
    {
        var userId = GetUserId();
        var query = _context.Operacoes
            .Include(op => op.Faturamentos)
            .Where(op => op.UserId == userId)
            .AsQueryable();

        if (isAtiva.HasValue)
        {
            query = query.Where(op => op.IsAtiva == isAtiva.Value);
        }
        if (ano.HasValue)
        {
            query = query.Where(op => op.DataInicio.Year == ano.Value);
        }
        if (mes.HasValue)
        {
            query = query.Where(op => op.DataInicio.Month == mes.Value);
        }

        var operacoes = await query.ToListAsync();
        return Ok(operacoes);
    }

    [HttpPost] // POST /api/operacoes
    public async Task<IActionResult> CreateOperacao([FromBody] OperacaoDto operacaoDto)
    {
        var userId = GetUserId();
        var novaOperacao = new Operacao
        {
            Id = Guid.NewGuid(),
            Nome = operacaoDto.Nome,
            Descricao = operacaoDto.Descricao,
            Endereco = operacaoDto.Endereco,
            Moeda = operacaoDto.Moeda,
            MetaMensal = operacaoDto.MetaMensal,
            DataInicio = operacaoDto.DataInicio.ToUniversalTime(),
            DataFim = operacaoDto.DataFim?.ToUniversalTime(),
            IsAtiva = true,
            UserId = userId
        };

        await _context.Operacoes.AddAsync(novaOperacao);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetOperacoes), new { id = novaOperacao.Id }, novaOperacao);
    }

    [HttpPatch("{id}/desativar")] // PATCH /api/operacoes/{id}/desativar
    public async Task<IActionResult> DeactivateOperacao(Guid id)
    {
        var userId = GetUserId();
        var operacao = await _context.Operacoes.FirstOrDefaultAsync(op => op.Id == id && op.UserId == userId);

        if (operacao == null)
        {
            return NotFound("Operação não encontrada ou não pertence ao usuário.");
        }

        operacao.IsAtiva = false;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("{id}/projecao")]
    public async Task<IActionResult> UpdateProjecao(Guid id, [FromBody] decimal projecao)
    {
        // Nota: Este endpoint é para comunicação interna entre serviços.
        // Em um ambiente de produção, ele seria protegido por uma chave de API ou outro método.
        var operacao = await _context.Operacoes.FindAsync(id);

        if (operacao == null)
        {
            return NotFound();
        }

        operacao.ProjecaoFaturamento = projecao;
        await _context.SaveChangesAsync();

        return NoContent();
    }
    [HttpGet("{id}")] // GET /api/operacoes/{id}
public async Task<IActionResult> GetOperacaoById(Guid id)
{
    var userId = GetUserId();
    var operacao = await _context.Operacoes
        .Include(op => op.Faturamentos) // Importante incluir os faturamentos!
        .FirstOrDefaultAsync(op => op.Id == id && op.UserId == userId);

    if (operacao == null)
    {
        return NotFound();
    }

    return Ok(operacao);
    }
}