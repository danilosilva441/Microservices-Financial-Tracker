using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BillingService.Data;
using BillingService.DTO;
using BillingService.Models;

namespace BillingService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Protege todos os endpoints neste controller
public class BillingController : ControllerBase
{
    private readonly BillingDbContext _context;

    public BillingController(BillingDbContext context)
    {
        _context = context;
    }

    private Guid GetUserId()
    {
        // Este método busca o ID do usuário logado a partir do token JWT.
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null)
        {
            // Isso não deveria acontecer se o [Authorize] estiver funcionando, mas é uma boa prática.
            throw new InvalidOperationException("User ID not found in token.");
        }
        return Guid.Parse(userIdClaim);
    }

    // --- Endpoints de Operações ---

    [HttpGet("operacoes")]
    public async Task<IActionResult> GetOperacoes()
    {
        var userId = GetUserId();
        var operacoes = await _context.Operacoes
            .Where(op => op.UserId == userId)
            .ToListAsync();
        
        return Ok(operacoes);
    }

    [HttpPost("operacoes")]
    public async Task<IActionResult> CreateOperacao([FromBody] CreateOperacaoDto operacaoDto)
    {
        var userId = GetUserId();

        var novaOperacao = new Operacao
        {
            Id = Guid.NewGuid(),
            Descricao = operacaoDto.Descricao,
            Valor = operacaoDto.Valor,
            DataInicio = operacaoDto.DataInicio,
            DataFim = operacaoDto.DataFim,
            IsAtiva = true,
            UserId = userId // Vincula a operação ao usuário logado
        };

        await _context.Operacoes.AddAsync(novaOperacao);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetOperacoes), new { id = novaOperacao.Id }, novaOperacao);
    }

    [HttpPatch("operacoes/{id}/desativar")]
    public async Task<IActionResult> DeactivateOperacao(Guid id)
    {
        var userId = GetUserId();
        
        // Busca a operação pelo ID E pelo ID do usuário para garantir que um usuário não desative a operação de outro.
        var operacao = await _context.Operacoes.FirstOrDefaultAsync(op => op.Id == id && op.UserId == userId);

        if (operacao == null)
        {
            return NotFound("Operação não encontrada ou não pertence ao usuário.");
        }

        operacao.IsAtiva = false;
        await _context.SaveChangesAsync();

        return NoContent(); // Resposta padrão para sucesso sem conteúdo
    }

    // --- Endpoints de Metas ---

    [HttpGet("metas")]
    public async Task<IActionResult> GetMeta([FromQuery] int mes, [FromQuery] int ano)
    {
        var userId = GetUserId();
        var meta = await _context.Metas
            .FirstOrDefaultAsync(m => m.UserId == userId && m.Mes == mes && m.Ano == ano);

        if (meta == null)
        {
            return NotFound("Meta para este período não encontrada.");
        }

        return Ok(meta);
    }

    [HttpPost("metas")]
    public async Task<IActionResult> SetMeta([FromBody] MetaDto metaDto)
    {
        var userId = GetUserId();
        
        // Verifica se já existe uma meta para o período
        var metaExistente = await _context.Metas
            .FirstOrDefaultAsync(m => m.UserId == userId && m.Mes == metaDto.Mes && m.Ano == metaDto.Ano);

        if (metaExistente != null)
        {
            // Se existe, atualiza
            metaExistente.ValorAlvo = metaDto.ValorAlvo;
        }
        else
        {
            // Se não existe, cria uma nova
            var novaMeta = new Meta
            {
                Id = Guid.NewGuid(),
                Mes = metaDto.Mes,
                Ano = metaDto.Ano,
                ValorAlvo = metaDto.ValorAlvo,
                UserId = userId
            };
            await _context.Metas.AddAsync(novaMeta);
        }

        await _context.SaveChangesAsync();
        return Ok("Meta salva com sucesso.");
    }
}