using BillingService.Data;
using BillingService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace BillingService.Controllers;

[ApiController]
[Route("api/billingadmin")]
[Authorize(Roles = "Admin, Gerente")]
public class AdminController : ControllerBase
{
    private readonly BillingDbContext _context;
    private readonly ILogger<AdminController> _logger;

    public AdminController(BillingDbContext context, ILogger<AdminController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // Cache da tenant_id para a requisição
    private Guid? _tenant_id;
    private Guid TenantId => _tenant_id ??= GetTenantIdFromToken();

    #region Método auxiliar para extrair TenantId do token
    private Guid GetTenantIdFromToken()
    {
        var tenant_idClaim = User.FindFirst("tenant_id")?.Value;

        if (string.IsNullOrEmpty(tenant_idClaim) ||
            !Guid.TryParse(tenant_idClaim, out var tenant_id))
        {
            _logger.LogWarning("Tenant ID não encontrado ou inválido no token para o usuário {UserId}",
                User.FindFirstValue(ClaimTypes.NameIdentifier));
            throw new UnauthorizedAccessException("Tenant ID não autorizado.");
        }

        return tenant_id;
    }
    #endregion

    public record VincularUsuarioDto(Guid UserId, Guid UnidadeId);

    #region Vincular Usuário a Unidade
    /// <summary>
    /// Endpoint para vincular um usuário a uma unidade específica dentro do tenant
    /// </summary>
    [HttpPost("vincular-usuario-unidade")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> VincularUsuarioUnidade(
         [FromBody] VincularUsuarioDto vinculoDto,
         CancellationToken cancellationToken = default)
    {
        try
        {
            if (vinculoDto == null)
            {
                return BadRequest("DTO é obrigatório.");
            }
            // 1. Validação de Ids
            if (vinculoDto.UserId == Guid.Empty || vinculoDto.UnidadeId == Guid.Empty)
            {
                return BadRequest("UserId e UnidadeId são obrigatórios.");
            }

            // 2. CORREÇÃO PRINCIPAL: Força a leitura do TenantId AQUI.
            // Se o token for inválido, a exceção UnauthorizedAccessException estoura aqui,
            // fora do LINQ, e será capturada pelo catch correto abaixo.
            var tenant_id = TenantId;

            // Verifica se unidade existe
            var unidadeExiste = await _context.Unidades
                // Usa a variável local 'tenant_id' em vez da propriedade
                .AnyAsync(u => u.Id == vinculoDto.UnidadeId && u.TenantId == tenant_id,
                          cancellationToken);

            if (!unidadeExiste)
            {
                return NotFound("Unidade não encontrada ou não pertence a este tenant.");
            }

            // Verifica se vínculo já existe
            var vinculoExistente = await _context.UsuarioOperacoes
                // Usa a variável local 'tenant_id'
                .AnyAsync(uo => uo.UserId == vinculoDto.UserId
                             && uo.UnidadeId == vinculoDto.UnidadeId
                             && uo.TenantId == tenant_id,
                          cancellationToken);

            if (vinculoExistente)
            {
                return Conflict("Este usuário já está vinculado a esta unidade.");
            }

            // Cria o novo vínculo
            var vinculo = new UsuarioOperacao
            {
                Id = Guid.NewGuid(),
                UserId = vinculoDto.UserId,
                UnidadeId = vinculoDto.UnidadeId,
                TenantId = tenant_id // Usa a variável local
            };

            await _context.UsuarioOperacoes.AddAsync(vinculo, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Vínculo criado: UserId={UserId}, UnidadeId={UnidadeId}, TenantId={TenantId}",
                vinculo.UserId, vinculo.UnidadeId, vinculo.TenantId);

            return Ok(new
            {
                Message = "Vínculo criado com sucesso.",
                VinculoId = vinculo.Id,
                UserId = vinculo.UserId,
                UnidadeId = vinculo.UnidadeId
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            // Agora o fluxo cairá aqui corretamente!
            _logger.LogWarning(ex, "Falha de autorização ao vincular usuário");
            return Unauthorized(ex.Message);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Erro de banco de dados ao vincular usuário {UserId} à unidade {UnidadeId}",
                vinculoDto.UserId, vinculoDto.UnidadeId);
            return StatusCode(StatusCodes.Status500InternalServerError,
                "Erro ao salvar vínculo no banco de dados.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao vincular usuário {UserId} à unidade {UnidadeId}",
                vinculoDto.UserId, vinculoDto.UnidadeId);
            return StatusCode(StatusCodes.Status500InternalServerError,
                "Ocorreu um erro interno ao processar a solicitação.");
        }
    }
    #endregion

    #region Get Vínculos do Usuário
    /// <summary>
    /// Endpoint opcional para listar vínculos do usuário
    /// </summary>
    [HttpGet("usuario/{userId}/vinculos")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetVinculosUsuario(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
        {
            return BadRequest("UserId é obrigatório.");
        }

        var vinculos = await _context.UsuarioOperacoes
            .Where(uo => uo.UserId == userId && uo.TenantId == TenantId)
            .Include(uo => uo.Unidade) // Se houver navegação
            .Select(uo => new
            {
                uo.Id,
                uo.UnidadeId,
                UnidadeNome = uo.Unidade.Nome, // Ajuste conforme seu modelo

            })
            .ToListAsync(cancellationToken);

        if (!vinculos.Any())
        {
            return NotFound("Nenhum vínculo encontrado para este usuário.");
        }

        return Ok(vinculos);
    }
    #endregion
}