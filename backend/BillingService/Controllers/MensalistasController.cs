using BillingService.DTOs;
using BillingService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SharedKernel.Exceptions;
using BillingService.Services.Interfaces;
using BillingService.DTO;

namespace BillingService.Controllers;

[ApiController]
[Route("api/unidades/{unidadeId}/mensalistas")]
[Authorize(Roles = "Admin,Gerente,Financeiro")]
public class MensalistasController : ControllerBase
{
    private readonly IMensalistaService _mensalistaService;
    private readonly ILogger<MensalistasController> _logger;

    public MensalistasController(
        IMensalistaService mensalistaService, 
        ILogger<MensalistasController> logger)
    {
        _mensalistaService = mensalistaService;
        _logger = logger;
    }
    #region Método auxiliar para extrair UserId do token
    /// <summary>
    /// Extrai o UserId do token JWT do usuário autenticado
    /// </summary>
    /// <returns>Guid do UserId</returns>aleatorizar UserId { get; }aleatorizar UserId { get; }
    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim))
        {
            _logger.LogWarning("User ID not found in token for user: {Username}", 
                User.Identity?.Name);
            throw new UnauthorizedAccessException("User ID not found in token.");
        }

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            _logger.LogWarning("Invalid User ID format in token: {UserIdClaim}", userIdClaim);
            throw new UnauthorizedAccessException("Invalid User ID format.");
        }

        return userId;
    }
    #endregion

    #region GET - Obter Todos os Mensalistas
    /// <summary>
    /// Endpoint para obter todos os mensalistas de uma unidade específica
    /// </summary>
    /// <param name="unidadeId">ID da unidade</param>
    /// <returns>Lista de mensalistas</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<MensalistaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAll(Guid unidadeId)
    {
        try
        {
            var userId = GetUserId();
            _logger.LogInformation("Getting all mensalistas for operation {OperacaoId} by user {UserId}", 
                unidadeId, userId);
            
            var mensalistas = await _mensalistaService.GetAllMensalistasAsync(unidadeId, userId);
            return Ok(mensalistas);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Resource not found for operation {OperacaoId}", unidadeId);
            return NotFound(new { Message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access for operation {OperacaoId}", unidadeId);
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting mensalistas for operation {OperacaoId}", unidadeId);
            return StatusCode(500, new { Message = "An error occurred while processing your request." });
        }
    }
    #endregion

    #region POST - Criar Novo Mensalista
    /// <summary>
    /// Endpoint para criar um novo mensalista em uma unidade específica
    /// </summary>
    /// <param name="unidadeId">ID da unidade</param>
    /// <param name="mensalistaDto">Dados do mensalista a ser criado</param>
    /// <returns>Resultado da criação</returns>
    [HttpPost]
    [Authorize(Roles = "Admin, Gerente, Financeiro")]
    [ProducesResponseType(typeof(MensalistaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(
        Guid unidadeId, 
        [FromBody] CreateMensalistaDto mensalistaDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state for creating mensalista in operation {OperacaoId}", 
                unidadeId);
            return BadRequest(ModelState);
        }

        try
        {
            var userId = GetUserId();
            _logger.LogInformation(
                "Creating mensalista in operation {OperacaoId} by user {UserId}", 
                unidadeId, userId);
            
            var novoMensalista = await _mensalistaService.CreateMensalistaAsync(
                unidadeId, mensalistaDto, userId);

            return CreatedAtAction(
                nameof(GetAll), 
                new { unidadeId }, 
                novoMensalista);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error creating mensalista in operation {OperacaoId}", 
                unidadeId);
            return BadRequest(new { ex.Message, ex.Errors });
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Resource not found for operation {OperacaoId}", unidadeId);
            return NotFound(new { Message = ex.Message });
        }
        catch (BusinessException ex)
        {
            _logger.LogWarning(ex, "Business rule violation in operation {OperacaoId}", unidadeId);
            return Conflict(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating mensalista in operation {OperacaoId}", unidadeId);
            return StatusCode(500, new { Message = "An error occurred while creating mensalista." });
        }
    }
    #endregion

    #region PUT - Atualizar Mensalista
    /// <summary>
    /// Endpoint para atualizar um mensalista existente em uma unidade específica
    /// </summary>
    /// <param name="unidadeId">ID da unidade</param>
    /// <param name="mensalistaId">ID do mensalista a ser atualizado</param>
    /// <param name="mensalistaDto">Dados do mensalista a ser atualizado</param>
    /// <returns>Resultado da atualização</returns>
    [HttpPut("{mensalistaId}")]
    [Authorize(Roles = "Admin, Gerente, Financeiro")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid unidadeId, 
        Guid mensalistaId, 
        [FromBody] UpdateMensalistaDto mensalistaDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var userId = GetUserId();
            _logger.LogInformation(
                "Updating mensalista {MensalistaId} in operation {OperacaoId} by user {UserId}", 
                mensalistaId, unidadeId, userId);
            
            await _mensalistaService.UpdateMensalistaAsync(
                unidadeId, mensalistaId, mensalistaDto, userId);
            
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, 
                "Mensalista {MensalistaId} not found in operation {OperacaoId}", 
                mensalistaId, unidadeId);
            return NotFound(new { Message = ex.Message });
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, 
                "Validation error updating mensalista {MensalistaId}", mensalistaId);
            return BadRequest(new { ex.Message, ex.Errors });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Error updating mensalista {MensalistaId} in operation {OperacaoId}", 
                mensalistaId, unidadeId);
            return StatusCode(500, new { Message = "An error occurred while updating mensalista." });
        }
    }
    #endregion

    #region PATCH - Desativar Mensalista
    /// <summary>
    /// Endpoint para desativar um mensalista existente em uma unidade específica
    /// </summary>
    /// <param name="unidadeId">ID da unidade</param>
    /// <param name="mensalistaId">ID do mensalista a ser desativado</param>
    /// <returns>Resultado da desativação</returns>
    [HttpPatch("{mensalistaId}/desativar")]
    [Authorize(Roles = "Admin, Gerente, Financeiro")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deactivate(Guid unidadeId, Guid mensalistaId)
    {
        try
        {
            var userId = GetUserId();
            _logger.LogInformation(
                "Deactivating mensalista {MensalistaId} in operation {OperacaoId} by user {UserId}", 
                mensalistaId, unidadeId, userId);
            
            await _mensalistaService.DeactivateMensalistaAsync(unidadeId, mensalistaId, userId);
            
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, 
                "Mensalista {MensalistaId} not found for deactivation in operation {OperacaoId}", 
                mensalistaId, unidadeId);
            return NotFound(new { Message = ex.Message });
        }
        catch (BusinessException ex)
        {
            _logger.LogWarning(ex, 
                "Cannot deactivate mensalista {MensalistaId} in operation {OperacaoId}", 
                mensalistaId, unidadeId);
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Error deactivating mensalista {MensalistaId} in operation {OperacaoId}", 
                mensalistaId, unidadeId);
            return StatusCode(500, new { Message = "An error occurred while deactivating mensalista." });
        }
    }
    #endregion

    #region GET - Obter Mensalista por ID
    /// <summary>
    /// Endpoint para obter um mensalista específico por ID em uma unidade
    /// </summary>
    /// <param name="unidadeId">ID da unidade</param>
    /// <param name="mensalistaId">ID do mensalista</param>
    /// <returns>Detalhes do mensalista</returns>
    [HttpGet("{mensalistaId}")]
    [ProducesResponseType(typeof(MensalistaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid unidadeId, Guid mensalistaId)
    {
        try
        {
            var userId = GetUserId();
            _logger.LogInformation(
                "Getting mensalista {MensalistaId} from operation {OperacaoId} by user {UserId}", 
                mensalistaId, unidadeId, userId);
            
            var mensalista = await _mensalistaService.GetMensalistaByIdAsync(
                unidadeId, mensalistaId, userId);
            
            return Ok(mensalista);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, 
                "Mensalista {MensalistaId} not found in operation {OperacaoId}", 
                mensalistaId, unidadeId);
            return NotFound(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Error getting mensalista {MensalistaId} from operation {OperacaoId}", 
                mensalistaId, unidadeId);
            return StatusCode(500, new { Message = "An error occurred while retrieving mensalista." });
        }
    }
    #endregion
}