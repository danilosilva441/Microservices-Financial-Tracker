// Caminho: backend/BillingService/Controller/FaturamentoDiarioController.cs
using BillingService.DTOs;
using BillingService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BillingService.Controllers
{
    [ApiController]
    [Route("api/unidades/{unidadeId}/fechamentos")]
    [Authorize]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class FaturamentoDiarioController : ControllerBase
    {
        private readonly IFaturamentoDiarioService _service;
        private readonly ILogger<FaturamentoDiarioController> _logger;

        public FaturamentoDiarioController(
            IFaturamentoDiarioService service,
            ILogger<FaturamentoDiarioController> logger)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // --- Funções Helper ---
        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                _logger.LogWarning("User ID not found in token");
                throw new UnauthorizedAccessException("User ID not found in token.");
            }
            
            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                _logger.LogWarning("Invalid User ID format in token: {UserIdClaim}", userIdClaim);
                throw new UnauthorizedAccessException("Invalid User ID format.");
            }
            
            return userId;
        }

        private Guid GetTenantId()
        {
            var tenantIdClaim = User.FindFirst("tenantId")?.Value;
            if (string.IsNullOrEmpty(tenantIdClaim))
            {
                _logger.LogWarning("Tenant ID not found in token");
                throw new UnauthorizedAccessException("Tenant ID (tenantId) not found in token.");
            }
            
            if (!Guid.TryParse(tenantIdClaim, out var tenantId))
            {
                _logger.LogWarning("Invalid Tenant ID format in token: {TenantIdClaim}", tenantIdClaim);
                throw new UnauthorizedAccessException("Invalid Tenant ID format.");
            }
            
            return tenantId;
        }

        // --- Endpoints ---

        /// <summary>
        /// (Líder/Operador) Submete um novo fechamento de caixa.
        /// </summary>
        /// <param name="unidadeId">ID da unidade</param>
        /// <param name="dto">Dados do fechamento</param>
        /// <returns>Fechamento criado</returns>
        [HttpPost]
        [Authorize(Roles = "Admin, Gerente, Supervisor, Lider, Operador")]
        [ProducesResponseType(typeof(FaturamentoDiarioResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> SubmeterFechamento(
            [FromRoute] Guid unidadeId,
            [FromBody] FaturamentoDiarioCreateDto dto)
        {
            try
            {
                var userId = GetUserId();
                var tenantId = GetTenantId();
                
                _logger.LogInformation(
                    "Tentativa de submissão de fechamento - Unidade: {UnidadeId}, Usuário: {UserId}, Tenant: {TenantId}",
                    unidadeId, userId, tenantId);

                var (fechamento, errorMessage) = await _service.SubmeterFechamentoAsync(unidadeId, dto, userId, tenantId);

                if (errorMessage != null)
                {
                    return HandleErrorResponse(errorMessage);
                }

                if (fechamento == null)
                {
                    _logger.LogWarning("Fechamento retornado como nulo sem mensagem de erro");
                    return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno ao processar o fechamento.");
                }

                _logger.LogInformation(
                    "Fechamento criado com sucesso - ID: {FechamentoId}", 
                    fechamento.Id);

                return CreatedAtAction(
                    nameof(GetFechamentoById),
                    new { unidadeId = unidadeId, id = fechamento.Id },
                    fechamento);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Erro de autorização ao submeter fechamento");
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao submeter fechamento");
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um erro interno.");
            }
        }

        /// <summary>
        /// (Supervisor) Lista todos os fechamentos pendentes no seu Tenant.
        /// </summary>
        /// <returns>Lista de fechamentos pendentes</returns>
        [HttpGet("/api/fechamentos/pendentes")]
        [Authorize(Roles = "Admin, Gerente, Supervisor")]
        [ProducesResponseType(typeof(IEnumerable<FaturamentoDiarioResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFechamentosPendentes()
        {
            try
            {
                var tenantId = GetTenantId();
                _logger.LogDebug("Buscando fechamentos pendentes para o Tenant: {TenantId}", tenantId);
                
                var pendentes = await _service.GetFechamentosPendentesAsync(tenantId);
                return Ok(pendentes);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Erro de autorização ao buscar fechamentos pendentes");
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao buscar fechamentos pendentes");
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um erro interno.");
            }
        }

        /// <summary>
        /// (Supervisor) Atualiza/Aprova um fechamento existente.
        /// </summary>
        /// <param name="unidadeId">ID da unidade</param>
        /// <param name="id">ID do fechamento</param>
        /// <param name="dto">Dados de revisão</param>
        /// <returns>Fechamento atualizado</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Gerente, Supervisor")]
        [ProducesResponseType(typeof(FaturamentoDiarioResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RevisarFechamento(
            [FromRoute] Guid unidadeId,
            [FromRoute] Guid id,
            [FromBody] FaturamentoDiarioSupervisorUpdateDto dto)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    _logger.LogWarning("Tentativa de revisão com ID de fechamento vazio");
                    return BadRequest("ID do fechamento é inválido.");
                }

                var supervisorId = GetUserId();
                var tenantId = GetTenantId();

                _logger.LogInformation(
                    "Tentativa de revisão de fechamento - ID: {FechamentoId}, Supervisor: {SupervisorId}, Tenant: {TenantId}",
                    id, supervisorId, tenantId);

                var (fechamento, errorMessage) = await _service.RevisarFechamentoAsync(id, dto, supervisorId, tenantId);

                if (errorMessage != null)
                {
                    return HandleErrorResponse(errorMessage);
                }

                if (fechamento == null)
                {
                    _logger.LogWarning("Fechamento retornado como nulo sem mensagem de erro");
                    return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno ao processar a revisão.");
                }

                _logger.LogInformation(
                    "Fechamento revisado com sucesso - ID: {FechamentoId}", 
                    id);

                return Ok(fechamento);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Erro de autorização ao revisar fechamento");
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao revisar fechamento");
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um erro interno.");
            }
        }

        /// <summary>
        /// Lista todos os fechamentos de uma unidade.
        /// </summary>
        /// <param name="unidadeId">ID da unidade</param>
        /// <returns>Lista de fechamentos da unidade</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FaturamentoDiarioResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFechamentosDaUnidade([FromRoute] Guid unidadeId)
        {
            try
            {
                var tenantId = GetTenantId();
                _logger.LogDebug(
                    "Buscando fechamentos da unidade - Unidade: {UnidadeId}, Tenant: {TenantId}",
                    unidadeId, tenantId);
                
                var fechamentos = await _service.GetFechamentosPorUnidadeAsync(unidadeId, tenantId);
                return Ok(fechamentos);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Erro de autorização ao buscar fechamentos da unidade");
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao buscar fechamentos da unidade");
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um erro interno.");
            }
        }

        /// <summary>
        /// Obtém um fechamento específico por ID.
        /// </summary>
        /// <param name="unidadeId">ID da unidade</param>
        /// <param name="id">ID do fechamento</param>
        /// <returns>Fechamento encontrado</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(FaturamentoDiarioResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetFechamentoById(
            [FromRoute] Guid unidadeId,
            [FromRoute] Guid id)
        {
            try
            {
                var tenantId = GetTenantId();
                _logger.LogDebug(
                    "Buscando fechamento - ID: {FechamentoId}, Unidade: {UnidadeId}, Tenant: {TenantId}",
                    id, unidadeId, tenantId);
                
                var fechamento = await _service.GetFechamentoByIdAsync(id, tenantId);

                if (fechamento == null || fechamento.UnidadeId != unidadeId)
                {
                    _logger.LogWarning(
                        "Fechamento não encontrado ou não pertence à unidade - ID: {FechamentoId}, Unidade: {UnidadeId}",
                        id, unidadeId);
                    return NotFound();
                }

                return Ok(fechamento);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Erro de autorização ao buscar fechamento por ID");
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao buscar fechamento por ID");
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um erro interno.");
            }
        }

        // --- Métodos Auxiliares Privados ---

        private IActionResult HandleErrorResponse(string errorMessage)
        {
            if (string.IsNullOrEmpty(errorMessage))
            {
                _logger.LogWarning("Mensagem de erro vazia recebida");
                return BadRequest();
            }

            switch (errorMessage.ToLower())
            {
                case string s when s.Contains("já existe"):
                    _logger.LogWarning("Conflito ao processar requisição: {ErrorMessage}", errorMessage);
                    return Conflict(errorMessage);
                
                case string s when s.Contains("não encontrado") || s.Contains("não encontrada"):
                    _logger.LogWarning("Recurso não encontrado: {ErrorMessage}", errorMessage);
                    return NotFound(errorMessage);
                
                default:
                    _logger.LogWarning("Erro na requisição: {ErrorMessage}", errorMessage);
                    return BadRequest(errorMessage);
            }
        }
    }
}