// Caminho: backend/BillingService/Controller/UnidadeController.cs
using BillingService.DTOs;
using BillingService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using System.Net;

namespace BillingService.Controllers;

[ApiController]
[Route("api/unidades")]
[Authorize]
public class UnidadeController : ControllerBase
{
    private readonly IUnidadeService _unidadeService;
    private readonly ILogger<UnidadeController> _logger;

    public UnidadeController(
        IUnidadeService unidadeService,
        ILogger<UnidadeController> logger)
    {
        _unidadeService = unidadeService;
        _logger = logger;
    }
    
    #region Helpers para extrair informações do token JWT 
    /// <summary>
    /// Obtém o ID do usuário autenticado do token JWT
    /// </summary>
    /// <returns>UserId como Guid</returns>
    /// <exception cref="UnauthorizedAccessException">Lançada se o UserId não for encontrado ou inválido</exception>
    private Guid GetUserId()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrWhiteSpace(userIdClaim))
            {
                _logger.LogWarning("Claim 'NameIdentifier' não encontrada no token JWT");
                throw new UnauthorizedAccessException("Identificação do usuário não encontrada.");
            }

            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                _logger.LogError("Claim 'NameIdentifier' com formato inválido: {UserIdClaim}", userIdClaim);
                throw new UnauthorizedAccessException("Identificação do usuário em formato inválido.");
            }

            return userId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter UserId do token JWT");
            throw;
        }
    }
    #endregion

    #region Helpers para extrair TenantId do token JWT
    /// <summary>
    /// Obtém o TenantId do token JWT
    /// </summary>
    /// <returns>TenantId como Guid</returns>
    /// <exception cref="UnauthorizedAccessException">Lançada se o TenantId não for encontrado ou inválido</exception>
    /// <exception cref="InvalidOperationException">Lançada se o usuário for Admin/Sistema (TenantId nulo)</exception>
    /// <returns></returns>
    private Guid GetTenantId()
    {
        try
        {
            var tenant_idClaim = User.FindFirst("tenant_id")?.Value;

            if (string.IsNullOrWhiteSpace(tenant_idClaim))
            {
                _logger.LogWarning("Claim 'tenant_id' não encontrada no token JWT");
                throw new InvalidOperationException(
                    "Tenant ID não encontrado no token. Este endpoint requer privilégios de Gerente.");
            }

            if (!Guid.TryParse(tenant_idClaim, out var tenant_id))
            {
                _logger.LogError("Claim 'tenant_id' com formato inválido: {TenantIdClaim}", tenant_idClaim);
                throw new InvalidOperationException("Tenant ID em formato inválido.");
            }

            return tenant_id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter TenantId do token JWT");
            throw;
        }
    }
    #endregion

    #region Helpers para verificar permissões
    /// <summary>
    /// Verifica se o usuário é Admin/Sistema (TenantId nulo)
    /// </summary>
    /// <returns>True se for Admin, False caso contrário</returns>
    private bool IsAdmin()
    {
        var tenant_idClaim = User.FindFirst("tenant_id");
        return tenant_idClaim == null || string.IsNullOrWhiteSpace(tenant_idClaim.Value);
    }
    #endregion

    #region Endpoints de Unidade
    /// <summary>
    /// Obtém todas as unidades (Admin: todas, Gerente: apenas do seu tenant)
    /// </summary>
    /// <returns>Lista de unidades</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UnidadeDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetUnidades()
    {
        try
        {
            _logger.LogInformation("Iniciando consulta de unidades. Usuário é Admin: {IsAdmin}", IsAdmin());

            if (IsAdmin())
            {
                var todasUnidades = await _unidadeService.GetAllUnidadesAdminAsync();
                _logger.LogInformation("Admin consultou {Count} unidades", todasUnidades?.Count() ?? 0);
                return Ok(todasUnidades);
            }
            else
            {
                var tenant_id = GetTenantId();
                var unidades = await _unidadeService.GetAllUnidadesByTenantAsync(tenant_id);
                _logger.LogInformation("Gerente do tenant {TenantId} consultou {Count} unidades", 
                    tenant_id, unidades?.Count() ?? 0);
                return Ok(unidades);
            }
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Acesso não autorizado ao consultar unidades");
            return Unauthorized(new ProblemDetails
            {
                Title = "Acesso não autorizado",
                Detail = ex.Message,
                Status = (int)HttpStatusCode.Unauthorized
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Operação inválida ao consultar unidades");
            return BadRequest(new ProblemDetails
            {
                Title = "Operação inválida",
                Detail = ex.Message,
                Status = (int)HttpStatusCode.BadRequest
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao consultar unidades");
            return StatusCode((int)HttpStatusCode.InternalServerError, new ProblemDetails
            {
                Title = "Erro interno do servidor",
                Detail = "Ocorreu um erro ao processar sua solicitação.",
                Status = (int)HttpStatusCode.InternalServerError
            });
        }
    }
    #endregion

    #region Get Unidade by ID
    /// <summary>
    /// Obtém uma unidade específica por ID
    /// </summary>
    /// <param name="id">ID da unidade</param>
    /// <returns>Unidade encontrada</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UnidadeDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> GetUnidadeById(Guid id)
    {
        try
        {
            _logger.LogInformation("Consultando unidade com ID: {UnidadeId}", id);
            
            var tenant_id = GetTenantId();
            var unidade = await _unidadeService.GetUnidadeByIdAsync(id, tenant_id);
            
            if (unidade == null)
            {
                _logger.LogWarning("Unidade com ID {UnidadeId} não encontrada para o tenant {TenantId}", 
                    id, tenant_id);
                return NotFound(new ProblemDetails
                {
                    Title = "Unidade não encontrada",
                    Detail = $"A unidade com ID {id} não foi encontrada ou você não tem permissão para acessá-la.",
                    Status = (int)HttpStatusCode.NotFound
                });
            }

            return Ok(unidade);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Acesso não autorizado à unidade {UnidadeId}", id);
            return Unauthorized(new ProblemDetails
            {
                Title = "Acesso não autorizado",
                Detail = ex.Message,
                Status = (int)HttpStatusCode.Unauthorized
            });
        }
    }
    #endregion

    #region Create Unidade
    /// <summary>
    /// Cria uma nova unidade
    /// </summary>
    /// <param name="unidadeDto">Dados da unidade a ser criada</param>
    /// <returns>Unidade criada</returns>
    [HttpPost]
    [Authorize(Roles = "Admin, Gerente")]
    [ProducesResponseType(typeof(UnidadeDto), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> CreateUnidade([FromBody] UnidadeDto unidadeDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Dados inválidos para criação de unidade: {@ModelState}", 
                    ModelState.Values.SelectMany(v => v.Errors));
                return ValidationProblem(ModelState);
            }

            var userId = GetUserId();
            var tenant_Id = GetTenantId();
            
            _logger.LogInformation(
                "Criando nova unidade. Usuário: {UserId}, Tenant: {TenantId}, Dados: {@UnidadeDto}",
                userId, tenant_Id, unidadeDto);

            var novaUnidade = await _unidadeService.CreateUnidadeAsync(unidadeDto, userId, tenant_Id);
            
            _logger.LogInformation("Unidade criada com sucesso. ID: {UnidadeId}", novaUnidade.Id);
            
            return CreatedAtAction(
                nameof(GetUnidadeById),
                new { id = novaUnidade.Id, version = "2.0" },
                novaUnidade);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Argumento inválido ao criar unidade");
            return BadRequest(new ProblemDetails
            {
                Title = "Dados inválidos",
                Detail = ex.Message,
                Status = (int)HttpStatusCode.BadRequest
            });
        }
    }
    #endregion


    #region Update Unidade
    /// <summary>
    /// Atualiza uma unidade existente
    /// </summary>
    /// <param name="id">ID da unidade a ser atualizada</param>
    /// <param name="unidadeDto">Dados atualizados da unidade</param>
    /// <returns></returns>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin, Gerente")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> UpdateUnidade(Guid id, [FromBody] UpdateUnidadeDto unidadeDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var tenant_id = GetTenantId();
            
            _logger.LogInformation(
                "Atualizando unidade {UnidadeId}. Tenant: {TenantId}, Dados: {@UnidadeDto}",
                id, tenant_id, unidadeDto);

            var success = await _unidadeService.UpdateUnidadeAsync(id, unidadeDto, tenant_id);

            if (!success)
            {
                _logger.LogWarning(
                    "Falha ao atualizar unidade {UnidadeId}. Não encontrada ou sem permissão.",
                    id);
                return NotFound(new ProblemDetails
                {
                    Title = "Unidade não encontrada",
                    Detail = "A unidade não foi encontrada ou você não tem permissão para atualizá-la.",
                    Status = (int)HttpStatusCode.NotFound
                });
            }

            _logger.LogInformation("Unidade {UnidadeId} atualizada com sucesso", id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Argumento inválido ao atualizar unidade {UnidadeId}", id);
            return BadRequest(new ProblemDetails
            {
                Title = "Dados inválidos",
                Detail = ex.Message,
                Status = (int)HttpStatusCode.BadRequest
            });
        }
    }
    #endregion

    #region Deactivate Unidade
    /// <summary>
    /// Desativa uma unidade (soft delete)
    /// </summary>
    /// <param name="id">ID da unidade a ser desativada</param>
    /// <returns></returns>
    [HttpPatch("{id:guid}/desativar")]
    [Authorize(Roles = "Admin, Gerente")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> DeactivateUnidade(Guid id)
    {
        try
        {
            var tenant_id = GetTenantId();
            
            _logger.LogInformation("Desativando unidade {UnidadeId}. Tenant: {TenantId}", id, tenant_id);
            
            var success = await _unidadeService.DeactivateUnidadeAsync(id, tenant_id);
            
            if (!success)
            {
                _logger.LogWarning(
                    "Falha ao desativar unidade {UnidadeId}. Não encontrada ou sem permissão.",
                    id);
                return NotFound(new ProblemDetails
                {
                    Title = "Unidade não encontrada",
                    Detail = "A unidade não foi encontrada ou você não tem permissão para desativá-la.",
                    Status = (int)HttpStatusCode.NotFound
                });
            }

            _logger.LogInformation("Unidade {UnidadeId} desativada com sucesso", id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Operação inválida ao desativar unidade {UnidadeId}", id);
            return BadRequest(new ProblemDetails
            {
                Title = "Operação não permitida",
                Detail = ex.Message,
                Status = (int)HttpStatusCode.BadRequest
            });
        }
    }
    #endregion

    #region Delete Unidade
    /// <summary>
    /// Exclui permanentemente uma unidade (hard delete)
    /// </summary>
    /// <param name="id">ID da unidade a ser excluída</param>
    /// <returns></returns>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin, Gerente")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Conflict)]
    public async Task<IActionResult> DeleteUnidade(Guid id)
    {
        try
        {
            var tenant_id = GetTenantId();
            
            _logger.LogWarning("Excluindo permanentemente unidade {UnidadeId}. Tenant: {TenantId}", 
                id, tenant_id);
            
            var success = await _unidadeService.DeleteUnidadeAsync(id, tenant_id);
            
            if (!success)
            {
                _logger.LogWarning(
                    "Falha ao excluir unidade {UnidadeId}. Não encontrada ou sem permissão.",
                    id);
                return NotFound(new ProblemDetails
                {
                    Title = "Unidade não encontrada",
                    Detail = "A unidade não foi encontrada ou não pertence ao seu tenant.",
                    Status = (int)HttpStatusCode.NotFound
                });
            }

            _logger.LogWarning("Unidade {UnidadeId} excluída permanentemente", id);
            return NoContent();
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("dependências"))
        {
            _logger.LogWarning(ex, "Não foi possível excluir unidade {UnidadeId} devido a dependências", id);
            return Conflict(new ProblemDetails
            {
                Title = "Conflito na exclusão",
                Detail = ex.Message,
                Status = (int)HttpStatusCode.Conflict
            });
        }
    }
    #endregion

    #region Update Projecao Faturamento
    /// <summary>
    /// Atualiza a projeção de faturamento de uma unidade (apenas Admin)
    /// </summary>
    /// <param name="id">ID da unidade a ser atualizada</param>
    /// <param name="projecaoDto">Dados da nova projeção</param>
    /// <returns></returns>
    [HttpPatch("{id:guid}/projecao")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> UpdateProjecaoFaturamento(
        Guid id,
        [FromBody] ProjecaoDto projecaoDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            _logger.LogInformation(
                "Atualizando projeção da unidade {UnidadeId}. Nova projeção: {Projecao}",
                id, projecaoDto.ProjecaoFaturamento);

            var success = await _unidadeService.UpdateProjecaoAsync(id, projecaoDto.ProjecaoFaturamento);
            
            if (!success)
            {
                _logger.LogWarning("Unidade {UnidadeId} não encontrada para atualização de projeção", id);
                return NotFound(new ProblemDetails
                {
                    Title = "Unidade não encontrada",
                    Detail = $"A unidade com ID {id} não foi encontrada.",
                    Status = (int)HttpStatusCode.NotFound
                });
            }

            _logger.LogInformation("Projeção da unidade {UnidadeId} atualizada com sucesso", id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Valor de projeção inválido para unidade {UnidadeId}", id);
            return BadRequest(new ProblemDetails
            {
                Title = "Valor de projeção inválido",
                Detail = ex.Message,
                Status = (int)HttpStatusCode.BadRequest
            });
        }
    }
    #endregion
}