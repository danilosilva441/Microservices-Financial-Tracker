using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthService.Data;
using AuthService.Models;
using AuthService.DTO;

namespace AuthService.Controllers;

[ApiController]
[Route("api/[controller]")] // Rota será /api/token
public class TokenController : ControllerBase
{
    private readonly AuthDbContext _context;
    private readonly IConfiguration _configuration;

    public TokenController(AuthDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost] // POST /api/token
    public async Task<IActionResult> Login([FromBody] UserDto request)
    {
        var user = await _context.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Unauthorized("Credenciais inválidas.");
        }

        string token = GenerateJwtToken(user);
        return Ok(new { token });
    }

    private string GenerateJwtToken(User user)
    {
        var jwtKey = _configuration["Jwt:Key"];
        if (string.IsNullOrEmpty(jwtKey))
        {
            throw new InvalidOperationException("Chave JWT não está configurada.");
        }
        
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in user.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.Name));
        }

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(8),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}