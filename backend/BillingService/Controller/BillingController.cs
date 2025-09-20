using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BillingService.Data;
using BillingService.DTO;
using BillingService.Models;
using Microsoft.EntityFrameworkCore;

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

    // A MUDANÇA ESTÁ AQUI: Adicionamos .Include() para carregar os faturamentos
    var operacoes = await _context.Operacoes
        .Include(op => op.Faturamentos) // <-- INCLUA OS FATURAMENTOS RELACIONADOS
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
            MetaMensal = operacaoDto.MetaMensal,
            // Converte a data para o padrão Universal (UTC) antes de salvar
            DataInicio = operacaoDto.DataInicio.ToUniversalTime(), // <--- CORREÇÃO
                                                                   // Se DataFim não for nula, também converte para UTC
            DataFim = operacaoDto.DataFim?.ToUniversalTime(),     // <--- CORREÇÃO
            IsAtiva = true,
            UserId = userId
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

    [HttpPost("operacoes/{operacaoId}/faturamentos")]
    public async Task<IActionResult> AddFaturamento(Guid operacaoId, [FromBody] CreateFaturamentoDto faturamentoDto)
    {
        var userId = GetUserId();

        // 1. Verifica se a operação existe e pertence ao usuário logado (MUITO IMPORTANTE!)
        var operacao = await _context.Operacoes
            .FirstOrDefaultAsync(op => op.Id == operacaoId && op.UserId == userId);

        if (operacao == null)
        {
            return NotFound("A operação especificada não foi encontrada ou não pertence a este usuário.");
        }

        // 2. Cria a nova entidade de Faturamento
        var novoFaturamento = new Faturamento
        {
            Id = Guid.NewGuid(),
            Valor = faturamentoDto.Valor,
            Data = faturamentoDto.Data.ToUniversalTime(), // <--- CORREÇÃO
            OperacaoId = operacaoId
        };

        // 3. Salva no banco de dados
        await _context.Faturamentos.AddAsync(novoFaturamento);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(AddFaturamento), new { id = novoFaturamento.Id }, novoFaturamento);
    }
}