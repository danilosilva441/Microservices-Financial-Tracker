using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using AuthService.DTO;
using AuthService.Services.Interfaces;

namespace AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    #region Public Endpoints

    /// <summary>
    /// Regista um novo utilizador no sistema (público para Devs/Admins)
    /// </summary>
    /// <param name="request">Dados do utilizador a registar</param>
    /// <returns>Dados do utilizador criado ou mensagem de erro</returns>
    /// <response code="201">Utilizador criado com sucesso</response>
    /// <response code="400">Dados inválidos ou erro de validação</response>
    /// <response code="500">Erro interno do servidor relacionado ao perfil</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] UserDto request)
    {
        try
        {
            var result = await _userService.RegisterAsync(request);

            if (!result.Success)
            {
                return HandleServiceError(result);
            }

            return CreatedAtAction(nameof(Register), result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao registar utilizador: {Email}", request.Email);
            return StatusCode(500, "Ocorreu um erro interno ao processar o registo.");
        }
    }

    #endregion

    #region Tenant Management Endpoints

    /// <summary>
    /// Cria um novo utilizador dentro do tenant do gerente autenticado
    /// </summary>
    /// <remarks>
    /// Apenas utilizadores com role "Gerente" podem criar utilizadores no seu tenant.
    /// Os perfis permitidos são: Supervisor, Lider, Operador.
    /// </remarks>
    /// <param name="request">Dados do utilizador a criar</param>
    /// <returns>Dados do utilizador criado ou mensagem de erro</returns>
    /// <response code="201">Utilizador criado com sucesso</response>
    /// <response code="400">Dados inválidos ou erro de validação</response>
    /// <response code="401">Token de autenticação inválido ou ausente</response>
    /// <response code="403">Permissão negada ou perfil não autorizado</response>
    [HttpPost("tenant-user")]
    [Authorize(Roles = "Gerente")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateTenantUser([FromBody] CreateTenantUserDto request)
    {
        try
        {
            // Extrair e validar informações do token
            var claimsValidation = ValidateUserClaims();
            if (!claimsValidation.isValid)
            {
                return claimsValidation.errorResult!;
            }

            // Delegar lógica para o serviço
            var serviceResult = await _userService.CreateTenantUserAsync(
                request, 
                claimsValidation.managerId!.Value, 
                claimsValidation.tenantId!.Value
            );

            if (!serviceResult.Success)
            {
                return HandleServiceError(serviceResult);
            }

            return CreatedAtAction(nameof(CreateTenantUser), serviceResult.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar utilizador no tenant");
            return StatusCode(500, "Ocorreu um erro interno ao criar o utilizador.");
        }
    }

    #endregion

    #region Private Helper Methods

    /// <summary>
    /// Valida as claims do utilizador autenticado
    /// </summary>
    private (bool isValid, Guid? managerId, Guid? tenantId, IActionResult? errorResult) ValidateUserClaims()
    {
        // Extrair e validar ManagerId
        var managerIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(managerIdClaim) || !Guid.TryParse(managerIdClaim, out var managerId))
        {
            return (false, null, null, Unauthorized("Token inválido: ID do utilizador não encontrado ou formato incorreto."));
        }

        // Extrair TenantId (suporta múltiplos nomes de claim)
        var tenantIdValue = User.FindFirst("tenant_id")?.Value 
                          ?? User.FindFirst("tenantId")?.Value 
                          ?? User.FindFirst("TenantId")?.Value;

        // Validar TenantId
        if (string.IsNullOrEmpty(tenantIdValue) || !Guid.TryParse(tenantIdValue, out var tenantId))
        {
            _logger.LogWarning(
                "Falha ao criar utilizador: Gerente {ManagerId} sem TenantId válido. Claims disponíveis: {Claims}",
                managerId,
                string.Join(", ", User.Claims.Select(c => $"{c.Type}: {c.Value}")));

            return (false, null, null, BadRequest("Ação falhou. O Gerente não tem um TenantId associado ou ele é inválido."));
        }

        return (true, managerId, tenantId, null);
    }

    /// <summary>
    /// Processa erros retornados pelo serviço
    /// </summary>
    /// <param name="result">Resultado do serviço com erro</param>
    /// <returns>Resposta HTTP apropriada com base no tipo de erro</returns>
    private IActionResult HandleServiceError(AuthResult result)
    {
        if (string.IsNullOrEmpty(result.ErrorMessage))
        {
            return BadRequest("Erro desconhecido.");
        }

        // Mapear mensagens de erro para códigos HTTP apropriados
        if (result.ErrorMessage.Contains("Perfil", StringComparison.OrdinalIgnoreCase) || 
            result.ErrorMessage.Contains("perfil", StringComparison.OrdinalIgnoreCase) ||
            result.ErrorMessage.Contains("Permissão", StringComparison.OrdinalIgnoreCase) || 
            result.ErrorMessage.Contains("permissão", StringComparison.OrdinalIgnoreCase))
        {
            return StatusCode(403, result.ErrorMessage);
        }

        if (result.ErrorMessage.Contains("inexistente", StringComparison.OrdinalIgnoreCase) || 
            result.ErrorMessage.Contains("não encontrado", StringComparison.OrdinalIgnoreCase) ||
            result.ErrorMessage.Contains("não existe", StringComparison.OrdinalIgnoreCase))
        {
            return NotFound(result.ErrorMessage);
        }

        if (result.ErrorMessage.Contains("Perfil") && result.ErrorMessage.Contains("500"))
        {
            return StatusCode(500, result.ErrorMessage);
        }

        return BadRequest(result.ErrorMessage);
    }

    #endregion
}