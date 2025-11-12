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

    [HttpPost] // POST /api/token
    public async Task<IActionResult> Login([FromBody] UserDto request)
    {
        var result = await _authService.LoginAsync(request);

        if (!result.Success)
        {
            return Unauthorized(result.ErrorMessage);
        }

        return Ok(result.Data);
    }
}