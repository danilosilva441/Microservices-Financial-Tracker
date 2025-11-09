using Microsoft.AspNetCore.Mvc;
using AuthService.DTO;
using AuthService.Services; // Importa o novo serviço

namespace AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    // 1. Injeta a INTERFACE do serviço, não mais o DbContext
    private readonly IAuthService _authService;

    public UsersController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost] // POST /api/users
    public async Task<IActionResult> Register([FromBody] UserDto request)
    {
        // 2. DELEGA toda a lógica para o serviço
        var result = await _authService.RegisterAsync(request);

        // 3. O Controller apenas decide o tipo de resposta (HTTP)
        if (!result.Success)
        {
            // Retorna o erro específico (ex: "Usuário já existe" ou "Perfil não encontrado")
            if (result.ErrorMessage.Contains("Perfil"))
                return StatusCode(500, result.ErrorMessage);
            
            return BadRequest(result.ErrorMessage);
        }

        return StatusCode(201, result.Data);
    }
}