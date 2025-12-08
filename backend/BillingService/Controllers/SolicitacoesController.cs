using BillingService.DTOs;
using BillingService.Models;
using BillingService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace BillingService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SolicitacoesController : ControllerBase
{
    private readonly ISolicitacaoService _service;
    private readonly ILogger<SolicitacoesController> _logger;

    public SolicitacoesController(
        ISolicitacaoService service,
        ILogger<SolicitacoesController> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            _logger.LogWarning("ID do usuário não encontrado ou inválido no token JWT");
            throw new UnauthorizedAccessException("Usuário não autenticado");
        }
        
        return userId;
    }

    private string? GetUserRole()
    {
        return User.FindFirstValue(ClaimTypes.Role);
    }

    [HttpPost]
    [ProducesResponseType(typeof(SolicitacaoAjuste), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CriarSolicitacao([FromBody] CriarSolicitacaoDto dto)
    {
        try
        {
            var userId = GetUserId();
            
            _logger.LogInformation(
                "Usuário {UserId} está criando uma solicitação para FaturamentoParcialId: {FaturamentoParcialId}",
                userId, dto.FaturamentoParcialId);

            var novaSolicitacao = new SolicitacaoAjuste
            {
                FaturamentoParcialId = dto.FaturamentoParcialId,
                Tipo = dto.Tipo,
                Motivo = dto.Motivo,
                DadosAntigos = dto.DadosAntigos,
                DadosNovos = dto.DadosNovos,
                Status = "PENDENTE"
            };

            var solicitacaoCriada = await _service.CriarSolicitacaoAsync(novaSolicitacao, userId);
            
            _logger.LogInformation(
                "Solicitação {SolicitacaoId} criada com sucesso por usuário {UserId}",
                solicitacaoCriada.Id, userId);

            return CreatedAtAction(
                nameof(GetSolicitacaoPorId),
                new { id = solicitacaoCriada.Id },
                solicitacaoCriada);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Falha na autenticação ao criar solicitação");
            return Unauthorized(new { Message = ex.Message });
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Dados inválidos para criação de solicitação");
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao criar solicitação");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { Message = "Ocorreu um erro interno ao processar sua solicitação" });
        }
    }

    [HttpGet]
    [Authorize(Roles = "Admin, Gerente")]
    [ProducesResponseType(typeof(IEnumerable<SolicitacaoAjuste>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetSolicitacoes(
        [FromQuery] string? status = null,
        [FromQuery] string? tipo = null)
    {
        try
        {
            var role = GetUserRole();
            _logger.LogInformation(
                "Usuário com role {Role} está listando solicitações. Filtros: Status={Status}, Tipo={Tipo}",
                role, status, tipo);

            var solicitacoes = await _service.GetSolicitacoesAsync();
            
            // Aplicar filtros em memória
            var filteredSolicitacoes = solicitacoes.AsQueryable();
            
            if (!string.IsNullOrEmpty(status))
            {
                filteredSolicitacoes = filteredSolicitacoes.Where(s => s.Status == status);
            }
            
            if (!string.IsNullOrEmpty(tipo))
            {
                filteredSolicitacoes = filteredSolicitacoes.Where(s => s.Tipo == tipo);
            }
            
            return Ok(filteredSolicitacoes.ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar solicitações");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { Message = "Erro ao recuperar solicitações" });
        }
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SolicitacaoAjuste), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetSolicitacaoPorId(Guid id)
    {
        try
        {
            var userId = GetUserId();
            var userRole = GetUserRole();
            
            _logger.LogInformation(
                "Usuário {UserId} (Role: {Role}) está acessando solicitação {SolicitacaoId}",
                userId, userRole, id);

            var solicitacoes = await _service.GetSolicitacoesAsync();
            var solicitacao = solicitacoes.FirstOrDefault(s => s.Id == id);

            if (solicitacao == null)
            {
                _logger.LogWarning("Solicitação {SolicitacaoId} não encontrada", id);
                return NotFound(new { Message = $"Solicitação {id} não encontrada" });
            }

            // Se não for Admin nem Gerente, retorna 403 (Forbidden)
            if (!User.IsInRole("Admin") && !User.IsInRole("Gerente"))
            {
                _logger.LogWarning(
                    "Usuário {UserId} não tem permissão para acessar solicitação {SolicitacaoId}",
                    userId, id);
                return Forbid();
            }

            return Ok(solicitacao);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Falha na autenticação ao acessar solicitação {SolicitacaoId}", id);
            return Unauthorized(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao acessar solicitação {SolicitacaoId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { Message = "Erro interno ao processar a solicitação" });
        }
    }

    [HttpPatch("{id:guid}/revisar")]
    [Authorize(Roles = "Admin, Gerente")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RevisarSolicitacao(
        Guid id,
        [FromBody] RevisarSolicitacaoDto revisaoDto)
    {
        try
        {
            var userId = GetUserId();
            
            _logger.LogInformation(
                "Usuário {UserId} está revisando solicitação {SolicitacaoId} com ação: {Acao}",
                userId, id, revisaoDto.Acao);

            // Chamada ao serviço - usando desconstrução direta
            var (success, errorMessage) = await _service.RevisarSolicitacaoAsync(id, revisaoDto.Acao, userId);

            if (!success)
            {
                _logger.LogWarning(
                    "Falha ao revisar solicitação {SolicitacaoId}: {Erro}",
                    id, errorMessage);

                return BadRequest(new { Message = errorMessage });
            }

            _logger.LogInformation(
                "Solicitação {SolicitacaoId} revisada com sucesso por usuário {UserId}",
                id, userId);

            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Falha na autenticação ao revisar solicitação");
            return Unauthorized(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao revisar solicitação {SolicitacaoId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { Message = "Erro interno ao processar a revisão" });
        }
    }

    [HttpGet("minhas")]
    [ProducesResponseType(typeof(IEnumerable<SolicitacaoAjuste>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMinhasSolicitacoes(
        [FromQuery] string? status = null)
    {
        try
        {
            var userId = GetUserId();
            
            _logger.LogDebug(
                "Usuário {UserId} está listando suas solicitações. Status: {Status}",
                userId, status);

            var todasSolicitacoes = await _service.GetSolicitacoesAsync();
            
            // Como não temos propriedade CriadoPor no modelo, retornamos todas
            // Se tiver essa propriedade no futuro, adicione o filtro:
            // var minhasSolicitacoes = todasSolicitacoes.Where(s => s.CriadoPor == userId);
            
            var minhasSolicitacoes = todasSolicitacoes.AsQueryable();
            
            if (!string.IsNullOrEmpty(status))
            {
                minhasSolicitacoes = minhasSolicitacoes.Where(s => s.Status == status);
            }
            
            return Ok(minhasSolicitacoes.ToList());
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Falha na autenticação ao listar solicitações do usuário");
            return Unauthorized(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar solicitações do usuário");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { Message = "Erro interno ao processar a solicitação" });
        }
    }
}

// DTO para revisão
public class RevisarSolicitacaoDto
{
    [Required(ErrorMessage = "A ação é obrigatória")]
    [RegularExpression("^(APROVAR|REJEITAR)$", ErrorMessage = "Ação deve ser 'APROVAR' ou 'REJEITAR'")]
    public string Acao { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "A justificativa não pode exceder 500 caracteres")]
    public string? Justificativa { get; set; }
}