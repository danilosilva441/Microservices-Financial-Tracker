using BillingService.DTOs;
using BillingService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SharedKernel.Exceptions;

namespace BillingService.Controllers;

[ApiController]
[Route("api/unidades/{unidadeId}/faturamentos-parciais")]
[Authorize]
public class FaturamentoParcialController : ControllerBase 
{
    private readonly IFaturamentoParcialService _faturamentoService;
    private readonly ILogger<FaturamentoParcialController> _logger;

    public FaturamentoParcialController(
        IFaturamentoParcialService faturamentoService,
        ILogger<FaturamentoParcialController> logger)
    {
        _faturamentoService = faturamentoService;
        _logger = logger;
    }

    #region Private Helpers

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null) 
            throw new System.InvalidOperationException("User ID not found in token.");
        return Guid.Parse(userIdClaim);
    }
    
    private Guid GetTenantId()
    {
        var tenantIdClaim = User.FindFirst("tenantId")?.Value;
        if (tenantIdClaim == null) 
            throw new System.InvalidOperationException("Tenant ID (tenantId) not found in token.");
        return Guid.Parse(tenantIdClaim);
    }

    #endregion

    #region POST - Criar Faturamento Parcial

    [HttpPost]
    public async Task<IActionResult> AddFaturamentoParcial(
        Guid unidadeId, 
        [FromBody] FaturamentoParcialCreateDto faturamentoDto)
    {
        try
        {
            var userId = GetUserId();
            var tenantId = GetTenantId();

            var novoFaturamento = await _faturamentoService.AddFaturamentoAsync(
                unidadeId, faturamentoDto, userId, tenantId);

            _logger.LogInformation(
                "Faturamento parcial criado com sucesso: {FaturamentoId} para unidade {UnidadeId}", 
                novoFaturamento.Id, unidadeId);

            return Ok(novoFaturamento);
        }
        catch (UnidadeAccessDeniedException ex)
        {
            _logger.LogWarning(
                "Acesso negado ao criar faturamento parcial: Usuário {UserId} na unidade {UnidadeId}", 
                GetUserId(), unidadeId);
            return Forbid(ex.Message);
        }
        catch (FaturamentoOverlapException ex)
        {
            _logger.LogWarning(
                "Sobreposição detectada ao criar faturamento parcial na unidade {UnidadeId}", 
                unidadeId);
            return Conflict(new { error = ex.Message });
        }
        catch (BusinessRuleException ex)
        {
            _logger.LogWarning(
                "Regra de negócio violada ao criar faturamento parcial na unidade {UnidadeId}: {Error}", 
                unidadeId, ex.Message);
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex, "Erro interno ao criar faturamento parcial na unidade {UnidadeId}", 
                unidadeId);
            return StatusCode(500, new { error = "Erro interno no servidor" });
        }
    }

    #endregion

    #region PUT - Atualizar Faturamento Parcial

    [HttpPut("{faturamentoId}")]
    [Authorize(Roles = "Admin, Gerente")]
    public async Task<IActionResult> UpdateFaturamentoParcial(
        Guid unidadeId, 
        Guid faturamentoId, 
        [FromBody] FaturamentoParcialUpdateDto faturamentoDto)
    {
        try
        {
            var userId = GetUserId();
            var tenantId = GetTenantId();

            await _faturamentoService.UpdateFaturamentoAsync(
                unidadeId, faturamentoId, faturamentoDto, userId, tenantId);

            _logger.LogInformation(
                "Faturamento parcial atualizado com sucesso: {FaturamentoId} na unidade {UnidadeId}", 
                faturamentoId, unidadeId);

            return NoContent();
        }
        catch (FaturamentoParcialNotFoundException ex)
        {
            _logger.LogWarning(
                "Faturamento parcial não encontrado para atualização: {FaturamentoId}", 
                faturamentoId);
            return NotFound(new { error = ex.Message });
        }
        catch (UnidadeAccessDeniedException ex)
        {
            _logger.LogWarning(
                "Acesso negado ao atualizar faturamento parcial: Usuário {UserId} no faturamento {FaturamentoId}", 
                GetUserId(), faturamentoId);
            return Forbid(ex.Message);
        }
        catch (FaturamentoOverlapException ex)
        {
            _logger.LogWarning(
                "Sobreposição detectada ao atualizar faturamento parcial {FaturamentoId}", 
                faturamentoId);
            return Conflict(new { error = ex.Message });
        }
        catch (BusinessRuleException ex)
        {
            _logger.LogWarning(
                "Regra de negócio violada ao atualizar faturamento parcial {FaturamentoId}: {Error}", 
                faturamentoId, ex.Message);
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex, "Erro interno ao atualizar faturamento parcial {FaturamentoId}", 
                faturamentoId);
            return StatusCode(500, new { error = "Erro interno no servidor" });
        }
    }

    #endregion

    #region DELETE - Excluir Faturamento Parcial

    [HttpDelete("{faturamentoId}")]
    [Authorize(Roles = "Admin, Gerente")]
    public async Task<IActionResult> DeleteFaturamentoParcial(
        Guid unidadeId, 
        Guid faturamentoId)
    {
        try
        {
            var userId = GetUserId();
            var tenantId = GetTenantId();

            await _faturamentoService.DeleteFaturamentoAsync(unidadeId, faturamentoId, userId, tenantId);

            _logger.LogInformation(
                "Faturamento parcial excluído com sucesso: {FaturamentoId} da unidade {UnidadeId}", 
                faturamentoId, unidadeId);

            return NoContent();
        }
        catch (FaturamentoParcialNotFoundException ex)
        {
            _logger.LogWarning(
                "Faturamento parcial não encontrado para exclusão: {FaturamentoId}", 
                faturamentoId);
            return NotFound(new { error = ex.Message });
        }
        catch (UnidadeAccessDeniedException ex)
        {
            _logger.LogWarning(
                "Acesso negado ao excluir faturamento parcial: Usuário {UserId} no faturamento {FaturamentoId}", 
                GetUserId(), faturamentoId);
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex, "Erro interno ao excluir faturamento parcial {FaturamentoId}", 
                faturamentoId);
            return StatusCode(500, new { error = "Erro interno no servidor" });
        }
    }

    #endregion

    #region PATCH - Desativar Faturamento Parcial

    [HttpPatch("{faturamentoId}/desativar")]
    [Authorize(Roles = "Admin, Gerente, Supervisor")]
    public async Task<IActionResult> DeactivateFaturamentoParcial(
        Guid unidadeId, 
        Guid faturamentoId)
    {
        try
        {
            var userId = GetUserId();
            var tenantId = GetTenantId();

            await _faturamentoService.DeactivateFaturamentoAsync(unidadeId, faturamentoId, userId, tenantId);

            _logger.LogInformation(
                "Faturamento parcial desativado com sucesso: {FaturamentoId} da unidade {UnidadeId}", 
                faturamentoId, unidadeId);

            return NoContent();
        }
        catch (FaturamentoParcialNotFoundException ex)
        {
            _logger.LogWarning(
                "Faturamento parcial não encontrado para desativação: {FaturamentoId}", 
                faturamentoId);
            return NotFound(new { error = ex.Message });
        }
        catch (UnidadeAccessDeniedException ex)
        {
            _logger.LogWarning(
                "Acesso negado ao desativar faturamento parcial: Usuário {UserId} no faturamento {FaturamentoId}", 
                GetUserId(), faturamentoId);
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex, "Erro interno ao desativar faturamento parcial {FaturamentoId}", 
                faturamentoId);
            return StatusCode(500, new { error = "Erro interno no servidor" });
        }
    }

    #endregion

    #region GET - Buscar Faturamentos (Método Adicional)

    [HttpGet]
    public async Task<IActionResult> GetFaturamentosPorUnidadeEData(
        Guid unidadeId,
        [FromQuery] DateOnly data)
    {
        try
        {
            var tenantId = GetTenantId();

            var faturamentos = await _faturamentoService.GetFaturamentosPorUnidadeEDataAsync(
                unidadeId, data, tenantId);

            _logger.LogDebug(
                "Retornados {Count} faturamentos parciais para unidade {UnidadeId} na data {Data}", 
                faturamentos.Count(), unidadeId, data);

            return Ok(faturamentos);
        }
        catch (UnidadeAccessDeniedException ex)
        {
            _logger.LogWarning(
                "Acesso negado ao buscar faturamentos: Usuário {UserId} na unidade {UnidadeId}", 
                GetUserId(), unidadeId);
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex, "Erro interno ao buscar faturamentos parciais na unidade {UnidadeId} na data {Data}", 
                unidadeId, data);
            return StatusCode(500, new { error = "Erro interno no servidor" });
        }
    }

    [HttpGet("{faturamentoId}")]
    public async Task<IActionResult> GetFaturamentoPorId(
        Guid unidadeId,
        Guid faturamentoId)
    {
        try
        {
            var tenantId = GetTenantId();

            // Você precisaria adicionar este método na interface e service
            // var faturamento = await _faturamentoService.GetFaturamentoByIdAsync(faturamentoId, tenantId);
            
            // Por enquanto, retornar NotImplemented
            return StatusCode(501, new { error = "Funcionalidade em desenvolvimento" });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex, "Erro interno ao buscar faturamento parcial {FaturamentoId}", 
                faturamentoId);
            return StatusCode(500, new { error = "Erro interno no servidor" });
        }
    }

    #endregion
}