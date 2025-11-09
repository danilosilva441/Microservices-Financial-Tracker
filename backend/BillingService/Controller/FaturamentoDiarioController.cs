// Caminho: backend/BillingService/Controller/FaturamentoDiarioController.cs
using BillingService.DTOs;
using BillingService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BillingService.Controllers
{
    [ApiController]
    [Route("api/unidades/{unidadeId}/fechamentos")] // Rota v2.0
    [Authorize]
    public class FaturamentoDiarioController : ControllerBase
    {
        private readonly IFaturamentoDiarioService _service;

        public FaturamentoDiarioController(IFaturamentoDiarioService service)
        {
            _service = service;
        }

        // --- Funções Helper v2.0 ---
        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) throw new InvalidOperationException("User ID not found in token.");
            return Guid.Parse(userIdClaim);
        }

        private Guid GetTenantId()
        {
            var tenantIdClaim = User.FindFirst("tenantId")?.Value;
            if (tenantIdClaim == null) throw new InvalidOperationException("Tenant ID (tenantId) not found in token.");
            return Guid.Parse(tenantIdClaim);
        }

        // --- Endpoints do Fluxo ---

        /// <summary>
        /// (Líder/Operador) Submete um novo fechamento de caixa.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin, Gerente, Supervisor, Lider, Operador")] // Lider pode criar
        public async Task<IActionResult> SubmeterFechamento(Guid unidadeId, [FromBody] FaturamentoDiarioCreateDto dto)
        {
            var userId = GetUserId();
            var tenantId = GetTenantId();
            var (fechamento, errorMessage) = await _service.SubmeterFechamentoAsync(unidadeId, dto, userId, tenantId);

            if (errorMessage != null)
            {
                if (errorMessage.Contains("já existe"))
                    return Conflict(errorMessage); // Erro 409
                if (errorMessage.Contains("não encontrada"))
                    return NotFound(errorMessage); // Erro 404

                return BadRequest(errorMessage); // Erro 400
            }

            return CreatedAtAction(nameof(GetFechamentoById), new { unidadeId = unidadeId, id = fechamento!.Id }, fechamento);
        }

        /// <summary>
        /// (Supervisor) Lista todos os fechamentos pendentes no seu Tenant.
        /// </summary>
        [HttpGet("/api/fechamentos/pendentes")] // Rota separada para o Supervisor
        [Authorize(Roles = "Admin, Gerente, Supervisor")]
        public async Task<IActionResult> GetFechamentosPendentes()
        {
            var tenantId = GetTenantId();
            var pendentes = await _service.GetFechamentosPendentesAsync(tenantId);
            return Ok(pendentes);
        }

        /// <summary>
        /// (Supervisor) Atualiza/Aprova um fechamento existente.
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Gerente, Supervisor")] // Apenas Supervisor (ou superior) pode aprovar
        public async Task<IActionResult> RevisarFechamento(Guid unidadeId, Guid id, [FromBody] FaturamentoDiarioSupervisorUpdateDto dto)
        {
            var supervisorId = GetUserId();
            var tenantId = GetTenantId();

            // Validamos a 'unidadeId' da rota, embora o 'id' do fechamento seja o principal
            if (id == Guid.Empty) return BadRequest("ID do fechamento é inválido.");

            var (fechamento, errorMessage) = await _service.RevisarFechamentoAsync(id, dto, supervisorId, tenantId);

            if (errorMessage != null)
            {
                if (errorMessage.Contains("não encontrado"))
                    return NotFound(errorMessage); // Erro 404

                return BadRequest(errorMessage); // Erro 400
            }

            return Ok(fechamento);
        }

        /// <summary>
        /// Lista todos os fechamentos de uma unidade.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetFechamentosDaUnidade(Guid unidadeId)
        {
            var tenantId = GetTenantId();
            var fechamentos = await _service.GetFechamentosPorUnidadeAsync(unidadeId, tenantId);
            return Ok(fechamentos);
        }

        /// <summary>
        /// Pega um fechamento específico.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFechamentoById(Guid unidadeId, Guid id)
        {
            var tenantId = GetTenantId();
            var fechamento = await _service.GetFechamentoByIdAsync(id, tenantId);

            if (fechamento == null || fechamento.UnidadeId != unidadeId)
                return NotFound();

            return Ok(fechamento);
        }
    }
}
