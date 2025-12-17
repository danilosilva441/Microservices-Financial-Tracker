// Caminho: backend/BillingService/Controller/MetasController.cs
using BillingService.DTOs;
using BillingService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BillingService.Controllers;

[ApiController]
[Route("api/unidades/{unidadeId:guid}/metas")]
[Authorize]
public class MetasController : ControllerBase
{
    private readonly IMetaService _metaService;
    private readonly ILogger<MetasController> _logger;

    public MetasController(
        IMetaService metaService,
        ILogger<MetasController> logger)
    {
        _metaService = metaService ?? throw new ArgumentNullException(nameof(metaService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    #region Métodos Auxiliares
    /// <summary>
    /// Obtém o TenantId do usuário autenticado
    /// </summary>
    /// <returns>Guid do TenantId</returns>
    /// <param name="unidadeId">ID da unidade</param>
    /// <returns>Guid do TenantId</returns>
    private Guid GetTenantId()
    {
        var tenant_idClaim = User.FindFirst("tenant_id")?.Value 
            ?? throw new UnauthorizedAccessException("Tenant ID não encontrado no token de autenticação.");
        
        if (!Guid.TryParse(tenant_idClaim, out var tenant_id))
        {
            throw new UnauthorizedAccessException("Tenant ID inválido no token de autenticação.");
        }

        return tenant_id;
    }
    #endregion

    #region Validação de Parâmetros
    /// <summary>
    /// Valida os parâmetros de mês e ano
    /// </summary>
    /// <param name="mes">Mês</param>
    /// <param name="ano">Ano</param>
    /// <param name="errorMessage">Mensagem de erro, se houver</param>
    /// <returns>True se válidos, false caso contrário</returns>
    private bool ValidateMesAno(int mes, int ano, out string? errorMessage)
    {
        errorMessage = null;
        
        if (mes < 1 || mes > 12)
        {
            errorMessage = "O mês deve estar entre 1 e 12.";
            return false;
        }

        if (ano < 2000 || ano > DateTime.UtcNow.Year + 1)
        {
            errorMessage = $"O ano deve estar entre 2000 e {DateTime.UtcNow.Year + 1}.";
            return false;
        }

        return true;
    }
    #endregion

    #region GET - Listar Metas da Unidade
    /// <summary>
    /// Lista todas as metas de uma unidade específica.
    /// </summary>
    /// <param name="unidadeId">ID da unidade</param>
    /// <param name="tenant_id">ID do tenant</param>
    /// <response code="200">Retorna a lista de metas</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="403">Usuário não tem permissão para acessar esta unidade</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<MetaDto>), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> GetMetas(Guid unidadeId)
    {
        try
        {
            var tenant_id = GetTenantId();
            _logger.LogInformation(
                "Buscando metas para a unidade {UnidadeId} do tenant {TenantId}", 
                unidadeId, tenant_id);

            var metas = await _metaService.GetMetasAsync(unidadeId, tenant_id);
            
            _logger.LogInformation(
                "Encontradas {Count} metas para a unidade {UnidadeId}", 
                metas.Count(), unidadeId);
                
            return Ok(metas);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Acesso não autorizado para unidade {UnidadeId}", unidadeId);
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Erro ao buscar metas para a unidade {UnidadeId}", 
                unidadeId);
            return StatusCode(500, 
                "Ocorreu um erro interno ao processar sua solicitação.");
        }
    }
    #endregion

    #region GET - Obter Meta por Período
    /// <summary>
    /// Busca uma meta específica pelo período (mês/ano).
    /// </summary>
    /// <param name="unidadeId">ID da unidade</param>
    /// <param name="mes">Mês da meta (1-12)</param>
    /// <param name="ano">Ano da meta</param>
    /// <response code="200">Retorna a meta encontrada</response>
    /// <response code="400">Parâmetros inválidos</response>
    /// <response code="404">Meta não encontrada para o período</response>
    /// <response code="403">Usuário não tem permissão para acessar esta unidade</response>
    [HttpGet("periodo")]
    [ProducesResponseType(typeof(MetaDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetMetaPorPeriodo(
        Guid unidadeId, 
        [FromQuery] int mes, 
        [FromQuery] int ano)
    {
        try
        {
            // Validação dos parâmetros
            if (!ValidateMesAno(mes, ano, out var validationError))
            {
                return BadRequest(validationError);
            }

            var tenant_id = GetTenantId();
            _logger.LogInformation(
                "Buscando meta para unidade {UnidadeId}, período {Mes}/{Ano}, tenant {TenantId}",
                unidadeId, mes, ano, tenant_id);

            var meta = await _metaService.GetMetaAsync(unidadeId, mes, ano, tenant_id);

            if (meta == null)
            {
                _logger.LogWarning(
                    "Meta não encontrada para unidade {UnidadeId}, período {Mes}/{Ano}", 
                    unidadeId, mes, ano);
                return NotFound(new 
                { 
                    Message = "Meta não encontrada para o período especificado.",
                    UnidadeId = unidadeId,
                    Mes = mes,
                    Ano = ano
                });
            }

            return Ok(meta);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, 
                "Acesso não autorizado para unidade {UnidadeId}, período {Mes}/{Ano}", 
                unidadeId, mes, ano);
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Erro ao buscar meta para unidade {UnidadeId}, período {Mes}/{Ano}", 
                unidadeId, mes, ano);
            return StatusCode(500, 
                "Ocorreu um erro interno ao processar sua solicitação.");
        }
    }
    #endregion

    #region POST - Criar/Atualizar Meta
    /// <summary>
    /// Cria ou atualiza a meta para um período.
    /// </summary>
    /// <param name="unidadeId">ID da unidade</param>
    /// <param name="metaDto">Dados da meta</param>
    /// <response code="200">Meta criada/atualizada com sucesso</response>
    /// <response code="400">Dados inválidos ou erro na operação</response>
    /// <response code="403">Usuário não tem permissão para esta operação</response>
    /// <response code="404">Unidade não encontrada</response>
    [HttpPost]
    [Authorize(Roles = "Admin, Gerente")] // Mantendo as roles como string
    [ProducesResponseType(typeof(MetaDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> SetMeta(
        Guid unidadeId, 
        [FromBody] MetaDto metaDto)
    {
        try
        {
            // Validação do DTO
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("DTO inválido para criação/atualização de meta: {@ModelState}", 
                    ModelState);
                return BadRequest(ModelState);
            }

            // Validação do período
            if (!ValidateMesAno(metaDto.Mes, metaDto.Ano, out var validationError))
            {
                return BadRequest(validationError);
            }

            var tenant_id = GetTenantId();
            
            _logger.LogInformation(
                "Criando/atualizando meta para unidade {UnidadeId}, período {Mes}/{Ano}, tenant {TenantId}",
                unidadeId, metaDto.Mes, metaDto.Ano, tenant_id);

            var (metaSalva, errorMessage) = await _metaService.SetMetaAsync(unidadeId, metaDto, tenant_id);
            
            if (errorMessage != null)
            {
                _logger.LogWarning(
                    "Erro ao salvar meta para unidade {UnidadeId}: {ErrorMessage}", 
                    unidadeId, errorMessage);

                if (errorMessage.Contains("não encontrada", StringComparison.OrdinalIgnoreCase) ||
                    errorMessage.Contains("not found", StringComparison.OrdinalIgnoreCase))
                {
                    return NotFound(new { Message = errorMessage });
                }
                
                return BadRequest(new { Message = errorMessage });
            }

            _logger.LogInformation(
                "Meta salva com sucesso para unidade {UnidadeId}, período {Mes}/{Ano}. ID: {MetaId}",
                unidadeId, metaDto.Mes, metaDto.Ano, metaSalva?.Id);

            return Ok(metaSalva);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, 
                "Acesso não autorizado para criação/atualização de meta na unidade {UnidadeId}", 
                unidadeId);
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Erro ao criar/atualizar meta para unidade {UnidadeId}", 
                unidadeId);
            return StatusCode(500, 
                "Ocorreu um erro interno ao processar sua solicitação.");
        }
    }
    #endregion
}