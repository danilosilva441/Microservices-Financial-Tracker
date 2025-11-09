using BillingService.Data;
using BillingService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims; // 1. ADICIONAR ESTE USING

namespace BillingService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin, Gerente")] // 2. ATUALIZADO (para incluir o Role v2.0)
public class AdminController : ControllerBase
{
    private readonly BillingDbContext _context;

    public AdminController(BillingDbContext context)
    {
        _context = context;
    }

    // 3. ADICIONADO (Helper v2.0 para segurança)
    private Guid GetTenantId()
    {
        var tenantIdClaim = User.FindFirst("tenantId")?.Value;
        if (tenantIdClaim == null) throw new InvalidOperationException("Tenant ID (tenantId) not found in token.");
        return Guid.Parse(tenantIdClaim);
    }

    public class VincularDto
    {
        public Guid UserId { get; set; }
        
        // 4. MUDANÇA (v2.0)
        // Renomeado de OperacaoId para UnidadeId
        public Guid UnidadeId { get; set; }
    }

    [HttpPost("vincular-usuario-operacao")]
    public async Task<IActionResult> VincularUsuarioOperacao([FromBody] VincularDto vinculoDto)
    {
        var tenantId = GetTenantId(); // Pega o TenantId do Admin/Gerente

        var vinculo = new UsuarioOperacao
        {
            UserId = vinculoDto.UserId,
            // 5. MUDANÇA (v2.0) - Corrige o erro CS0117
            UnidadeId = vinculoDto.UnidadeId,
            TenantId = tenantId // Adiciona o TenantId ao novo vínculo
        };

        var jaExiste = await _context.UsuarioOperacoes
            // 6. MUDANÇA (v2.0) - Corrige o erro CS1061
            .AnyAsync(uo => uo.UserId == vinculo.UserId && 
                           uo.UnidadeId == vinculo.UnidadeId && 
                           uo.TenantId == tenantId);

        if (jaExiste)
        {
            return Conflict("Este usuário já está vinculado a esta operação.");
        }

        await _context.UsuarioOperacoes.AddAsync(vinculo);
        await _context.SaveChangesAsync();

        return Ok("Vínculo criado com sucesso.");
    }
}