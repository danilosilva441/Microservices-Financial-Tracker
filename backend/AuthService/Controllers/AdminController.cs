using AuthService.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize(Roles = "DEV")] // Em um sistema real, protegeríamos isso
public class AdminController : ControllerBase
{
    private readonly AuthDbContext _context;

    public AdminController(AuthDbContext context)
    {
        _context = context;
    }

    [HttpPost("promote-to-admin")]
    public async Task<IActionResult> PromoteToAdmin([FromBody] string userEmail)
    {
        var user = await _context.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Email == userEmail);

        if (user == null)
        {
            return NotFound("Usuário não encontrado.");
        }

        var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
        if (adminRole == null)
        {
            return StatusCode(500, "Perfil 'Admin' não encontrado no banco de dados.");
        }

        // Verifica se o usuário já é Admin para não adicionar de novo
        if (!user.Roles.Any(r => r.Name == "Admin"))
        {
            user.Roles.Add(adminRole);
            await _context.SaveChangesAsync();
        }

        return Ok($"Usuário {userEmail} foi promovido a Admin.");
    }
}