using AuthService.Services; // Importa o novo serviço
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    // 1. Injeta a INTERFACE do serviço
    private readonly IAuthService _authService;

    public AdminController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("promote-to-admin")]
    public async Task<IActionResult> PromoteToAdmin([FromBody] string userEmail)
    {
        // 2. DELEGA toda a lógica para o serviço
        var result = await _authService.PromoteToAdminAsync(userEmail);

        // 3. O Controller apenas decide o tipo de resposta (HTTP)
        if (!result.Success)
        {
            if (result.ErrorMessage.Contains("não encontrado"))
                return NotFound(result.ErrorMessage);
            
            return StatusCode(500, result.ErrorMessage);
        }

        return Ok(result.Data);
    }
}