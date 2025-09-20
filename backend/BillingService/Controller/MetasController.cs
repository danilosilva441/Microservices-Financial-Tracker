using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BillingService.Data;
using BillingService.DTO;
using BillingService.Models;

namespace BillingService.Controllers;

[ApiController]
[Route("api/[controller]")] // Rota será /api/metas
[Authorize]
public class MetasController : ControllerBase
{
    private readonly BillingDbContext _context;
    
    public MetasController(BillingDbContext context)
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

    [HttpGet] // GET /api/metas?mes=9&ano=2025
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

    [HttpPost] // POST /api/metas
    public async Task<IActionResult> SetMeta([FromBody] MetaDto metaDto)
    {
        var userId = GetUserId();
        var metaExistente = await _context.Metas
            .FirstOrDefaultAsync(m => m.UserId == userId && m.Mes == metaDto.Mes && m.Ano == metaDto.Ano);

        if (metaExistente != null)
        {
            metaExistente.ValorAlvo = metaDto.ValorAlvo;
        }
        else
        {
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