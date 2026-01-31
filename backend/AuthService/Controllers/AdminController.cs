// Caminho: backend/AuthService/Controllers/AdminController.cs
using AuthService.Services.Interfaces; // 1. MUDANÇA: Namespace corrigido
using AuthService.DTO; // 2. ADICIONADO: Para o novo DTO
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    // 3. MUDANÇA: Injeta o IUserService (onde a lógica de 'Promote' agora vive)
    private readonly IUserService _userService;

    public AdminController(IUserService userService) // <-- MUDANÇA AQUI
    {
        _userService = userService;
    }

    /// <summary>
    ///     Promove um utilizador existente ao papel de Admin
    /// </summary>
    /// <param name="request">Dados do utilizador a promover</param>
    /// returns>Dados do utilizador promovido ou mensagem de erro</returns>
    /// <response code="200">Utilizador promovido com sucesso</response>
    /// <response code="404">Utilizador não encontrado</response>
    /// <response code="500">Erro interno do servidor relacionado à promoção</response>
    [HttpPost("promote-to-admin")]
    // [Authorize(Roles = "Dev")] // TODO: Proteger este endpoint
    public async Task<IActionResult> PromoteToAdmin([FromBody] PromoteAdminDto request) // <-- MUDANÇA AQUI
    {
        // 4. MUDANÇA: Chama o _userService
        var result = await _userService.PromoteToAdminAsync(request.Email);

        if (!result.Success)
        {
            if (result.ErrorMessage.Contains("não encontrado"))
                return NotFound(result.ErrorMessage);
            
            return StatusCode(500, result.ErrorMessage);
        }

        return Ok(result.Data);
    }
    
}