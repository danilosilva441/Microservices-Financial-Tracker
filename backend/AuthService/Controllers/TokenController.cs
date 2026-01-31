// Caminho: backend/AuthService/Controllers/TokenController.cs
using Microsoft.AspNetCore.Mvc;
using AuthService.DTO;
using AuthService.Services.Interfaces; // 1. MUDANÇA: Namespace corrigido

namespace AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TokenController : ControllerBase
{
    // A injeção do IAuthService está correta (o Login ainda vive aqui)
    private readonly IAuthService _authService;

    public TokenController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    ///     Gera um token JWT para autenticação do utilizador
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost] // POST /api/token
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto request)
    {
        var result = await _authService.LoginAsync(request);

        if (!result.Success)
        {
            return Unauthorized(result.ErrorMessage);
        }

        return Ok(result.Data);
    }
}