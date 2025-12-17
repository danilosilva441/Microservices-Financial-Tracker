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

        #region Helper Functions
        //<summary>
        // Funções helper para extrair UserId e TenantId do token JWT
        // --- Funções Helper ---
        //</summary>
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
            var tenant_idClaim = User.FindFirst("tenant_id")?.Value;
            if (string.IsNullOrEmpty(tenant_idClaim))
            {
                _logger.LogWarning("Tenant ID not found in token");
                throw new UnauthorizedAccessException("Tenant ID (tenant_id) not found in token.");
            }

            if (!Guid.TryParse(tenant_idClaim, out var tenant_id))
            {
                _logger.LogWarning("Invalid Tenant ID format in token: {TenantIdClaim}", tenant_idClaim);
                throw new UnauthorizedAccessException("Invalid Tenant ID format.");
            }

            return tenant_id;
        }
        #endregion

        #region Submit Daily Closing
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
                var tenant_id = GetTenantId();

                _logger.LogInformation(
                    "Tentativa de submissão de fechamento - Unidade: {UnidadeId}, Usuário: {UserId}, Tenant: {TenantId}",
                    unidadeId, userId, tenant_id);

                var (fechamento, errorMessage) = await _service.SubmeterFechamentoAsync(unidadeId, dto, userId, tenant_id);

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
        #endregion

        #region Get Pending Closings
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
                var tenant_id = GetTenantId();
                _logger.LogDebug("Buscando fechamentos pendentes para o Tenant: {TenantId}", tenant_id);

                var pendentes = await _service.GetFechamentosPendentesAsync(tenant_id);
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
        #endregion

        #region Review Daily Closing
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
                var tenant_id = GetTenantId();

                _logger.LogInformation(
                    "Tentativa de revisão de fechamento - ID: {FechamentoId}, Supervisor: {SupervisorId}, Tenant: {TenantId}",
                    id, supervisorId, tenant_id);

                var (fechamento, errorMessage) = await _service.RevisarFechamentoAsync(id, dto, supervisorId, tenant_id);

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
        #endregion

        #region Get Unit Closings
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
                var tenant_id = GetTenantId();
                _logger.LogDebug(
                    "Buscando fechamentos da unidade - Unidade: {UnidadeId}, Tenant: {TenantId}",
                    unidadeId, tenant_id);

                var fechamentos = await _service.GetFechamentosPorUnidadeAsync(unidadeId, tenant_id);
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
        #endregion

        #region Get Closing by ID
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
                var tenant_id = GetTenantId();
                _logger.LogDebug(
                    "Buscando fechamento - ID: {FechamentoId}, Unidade: {UnidadeId}, Tenant: {TenantId}",
                    id, unidadeId, tenant_id);

                var fechamento = await _service.GetFechamentoByIdAsync(id, tenant_id);

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
        #endregion


        #region Get Closings by Date Range
        /// <summary>
        /// Obtém fechamentos por intervalo de datas.
        /// </summary>
        /// <param name="unidadeId">ID da unidade</param>
        /// <param name="dataInicio">Data de início</param>
        /// <param name="dataFim">Data de fim</param>
        /// <returns>Lista de fechamentos no intervalo</returns>
        [HttpGet("por-data")]
        [ProducesResponseType(typeof(IEnumerable<FaturamentoDiarioResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetFechamentosPorData(
            [FromRoute] Guid unidadeId,
            [FromQuery] DateTime dataInicio,
            [FromQuery] DateTime dataFim)
        {
            try
            {
                var tenant_id = GetTenantId();

                _logger.LogDebug(
                    "Buscando fechamentos por data - Unidade: {UnidadeId}, Tenant: {TenantId}, Data Início: {DataInicio}, Data Fim: {DataFim}",
                    unidadeId, tenant_id, dataInicio, dataFim);

                // Converta DateTime para DateOnly (se necessário)
                var dataInicioDateOnly = DateOnly.FromDateTime(dataInicio);
                var dataFimDateOnly = DateOnly.FromDateTime(dataFim);

                var fechamentos = await _service.GetFechamentosPorDataAsync(
                    unidadeId,
                    dataInicioDateOnly,
                    dataFimDateOnly,
                    tenant_id);

                if (fechamentos == null || !fechamentos.Any())
                {
                    return NotFound("Nenhum fechamento encontrado para o período especificado.");
                }

                // Mapeie corretamente baseado nas propriedades reais do seu modelo
                var response = fechamentos.Select(f => new FaturamentoDiarioResponseDto
                {
                    Id = f.Id,
                    Data = f.Data,
                });

                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Argumento inválido ao buscar fechamentos por data");
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Erro de autorização ao buscar fechamentos por data");
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao buscar fechamentos por data");
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um erro interno.");
            }
        }
        #endregion


        #region Get Closings Summary
        /// <summary>
        /// Obtém um resumo dos fechamentos em um intervalo de datas.
        /// </summary>
        /// <param name="unidadeId">ID da unidade</param>
        /// <param name="dataInicio">Data de início</param>
        /// <param name="dataFim">Data de fim</param>
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
        #endregion
    }
}