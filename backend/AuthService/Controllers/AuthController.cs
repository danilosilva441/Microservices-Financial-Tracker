using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthService.Data;
using AuthService.Models;

namespace AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(AuthDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    // POST /api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserDto request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        {
            return BadRequest("Usuário com este email já existe.");
        }

        // Gera o hash da senha usando BCrypt
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = passwordHash
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return StatusCode(201, "Usuário criado com sucesso.");
    }

    // POST /api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserDto request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null)
        {
            return Unauthorized("Credenciais inválidas."); // Não diga se o email ou a senha está errado
        }

        // Verifica se a senha fornecida corresponde ao hash salvo
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Unauthorized("Credenciais inválidas.");
        }

        // Se a autenticação for bem-sucedida, gera o token JWT
        string token = GenerateJwtToken(user);

        return Ok(new { token });
    }

    private string GenerateJwtToken(User user)
    {
        // Pega a chave secreta do appsettings.json
        var jwtKey = _configuration["Jwt:Key"];
        if (string.IsNullOrEmpty(jwtKey))
        {
            throw new InvalidOperationException("Chave JWT não está configurada no appsettings.json");
        }
        
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // Claims são as "informações" que o token carrega sobre o usuário
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), // Subject (ID do usuário)
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // JWT ID, para identificação única do token
        };

        // Monta o token
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(8), // Tempo de expiração do token
            signingCredentials: credentials);

        // Escreve o token como uma string
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

// DTO (Data Transfer Object) para receber os dados de registro/login
public class UserDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}

// Modifique seu Model User.cs para não ter a dependência do DTO
// AuthService/Models/User.cs
// public required string PasswordHash { get; set; }