using Microsoft.AspNetCore.Mvc;
using AuthService.DTO;
using AuthService.Services; // Importa o novo serviço

namespace AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TokenController : ControllerBase
{
    // 1. Injeta a INTERFACE do serviço
    private readonly IAuthService _authService;

    public TokenController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost] // POST /api/token
    public async Task<IActionResult> Login([FromBody] UserDto request)
    {
        // 2. DELEGA toda a lógica de login e geração de token para o serviço
        var result = await _authService.LoginAsync(request);

        // 3. O Controller apenas decide o tipo de resposta (HTTP)
        if (!result.Success)
        {
            return Unauthorized(result.ErrorMessage);
        }

        return Ok(result.Data);
    }
}