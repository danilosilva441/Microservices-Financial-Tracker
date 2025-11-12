using Microsoft.AspNetCore.Mvc;
using AuthService.DTO;
using AuthService.Services.Interfaces; // 1. IMPORTANTE: Usando o namespace das interfaces
using Microsoft.AspNetCore.Authorization; // 2. IMPORTANTE: Para [Authorize]
using System.Security.Claims; // 3. IMPORTANTE: Para o Claims

namespace AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService) 
    {
        _userService = userService;
    }

    // --- Endpoint de Registo Público (para Devs/Admins) ---
    [HttpPost("register")] // Rota mudada de "/" para "/register"
    public async Task<IActionResult> Register([FromBody] UserDto request)
    {
        var result = await _userService.RegisterAsync(request);

        if (!result.Success)
        {
            if (result.ErrorMessage.Contains("Perfil"))
                return StatusCode(500, result.ErrorMessage);
            
            return BadRequest(result.ErrorMessage);
        }

        return StatusCode(201, result.Data);
    }

    // --- NOSSO NOVO ENDPOINT (Tarefa 7) ---

    /// <summary>
    /// Endpoint para um 'Gerente' (logado) criar um novo funcionário
    /// (Supervisor, Lider, Operador) dentro do seu próprio Tenant.
    /// </summary>
    [HttpPost("tenant-user")]
    [Authorize(Roles = "Gerente")] // 4. SÓ O GERENTE PODE CHAMAR
    public async Task<IActionResult> CreateTenantUser([FromBody] CreateTenantUserDto request)
    {
        // 5. Pegamos os dados do Gerente (que está a fazer a chamada) a partir do token
        var managerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var tenantIdClaim = User.FindFirst("tenantId")?.Value;

        if (tenantIdClaim == null)
        {
            return Unauthorized("Ação falhou. O Gerente não tem um TenantId associado.");
        }
        
        var tenantId = Guid.Parse(tenantIdClaim);

        // 6. Delegamos a lógica para o serviço
        var result = await _userService.CreateTenantUserAsync(request, managerId, tenantId);

        if (!result.Success)
        {
            if (result.ErrorMessage.Contains("Perfil") || result.ErrorMessage.Contains("Permissão"))
                return StatusCode(403, result.ErrorMessage); // Forbidden
            
            return BadRequest(result.ErrorMessage);
        }

        return StatusCode(201, result.Data);
    }
}