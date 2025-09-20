using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthService.Data;
using AuthService.DTO;
using AuthService.Models;

namespace AuthService.Controllers;

[ApiController]
[Route("api/[controller]")] // Rota será /api/users
public class UsersController : ControllerBase
{
    private readonly AuthDbContext _context;

    public UsersController(AuthDbContext context)
    {
        _context = context;
    }

    [HttpPost] // POST /api/users
    public async Task<IActionResult> Register([FromBody] UserDto request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        {
            return BadRequest("Usuário com este email já existe.");
        }

        string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User");
        if (userRole == null)
        {
            return StatusCode(500, "Perfil 'User' padrão não encontrado.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = passwordHash,
            Roles = new List<Role> { userRole }
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Retorna um objeto anônimo para não expor o modelo User completo com o hash da senha
        return StatusCode(201, new { UserId = user.Id, Email = user.Email });
    }
}