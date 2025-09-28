using BillingService.Data;
using BillingService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BillingService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")] // Apenas Admins podem acessar este controller
public class AdminController : ControllerBase
{
    private readonly BillingDbContext _context;

    public AdminController(BillingDbContext context)
    {
        _context = context;
    }

    // DTO para receber os dados do vínculo
    public class VincularDto
    {
        public Guid UserId { get; set; }
        public Guid OperacaoId { get; set; }
    }

    [HttpPost("vincular-usuario-operacao")]
    public async Task<IActionResult> VincularUsuarioOperacao([FromBody] VincularDto vinculoDto)
    {
        var vinculo = new UsuarioOperacao
        {
            UserId = vinculoDto.UserId,
            OperacaoId = vinculoDto.OperacaoId
        };

        var jaExiste = await _context.UsuarioOperacoes
            .AnyAsync(uo => uo.UserId == vinculo.UserId && uo.OperacaoId == vinculo.OperacaoId);

        if (jaExiste)
        {
            return Conflict("Este usuário já está vinculado a esta operação.");
        }

        await _context.UsuarioOperacoes.AddAsync(vinculo);
        await _context.SaveChangesAsync();

        return Ok("Vínculo criado com sucesso.");
    }
}