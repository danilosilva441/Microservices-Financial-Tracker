using BillingService.DTOs.Shifts;
using BillingService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BillingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Segurança JWT
    public class ShiftsController : ControllerBase
    {
        private readonly IShiftService _service;

        public ShiftsController(IShiftService service)
        {
            _service = service;
        }

        private Guid GetTenantId() => Guid.Parse(User.FindFirst("tenantId")?.Value ?? throw new Exception("TenantId não encontrado"));

        #region Work Schedule Templates and Shift Management
        /// <summary>
        /// Cria um novo template de escala de trabalho
        /// </summary>
        /// <param name="dto">Dados do template de escala</param>
        /// <returns>Template criado</returns>
        [HttpPost("templates")]
        public async Task<IActionResult> CreateTemplate([FromBody] CreateWorkScheduleDto dto)
        {
            var result = await _service.CreateWorkScheduleAsync(dto, GetTenantId());
            if (!result.Success) return BadRequest(result.ErrorMessage);
            return Created("", result.Value);
        }
        #endregion

        #region Shift Generation and Retrieval
        /// <summary>
        /// Gera escalas de trabalho para uma unidade com base em um template e intervalo de
        /// datas, atribuindo usuários específicos.
        /// </summary>
        /// <param name="request">Dados para geração de escalas</param>
        /// <returns>Resultado da operação</returns>
        [HttpPost("generate")]
        public async Task<IActionResult> GenerateShifts([FromBody] GenerateRequest request)
        {
            var result = await _service.GenerateShiftsAsync(
                request.UnidadeId, 
                request.TemplateId, 
                request.StartDate, 
                request.EndDate, 
                request.UserIds, 
                GetTenantId());

            if (!result.Success) return BadRequest(result.ErrorMessage);
            return Ok("Escalas geradas com sucesso.");
        }
        #endregion

        #region Shift Retrieval and Break Management
        /// <summary>
        /// Obtém as escalas de trabalho para uma unidade dentro de um intervalo de datas.
        /// </summary>
        /// <param name="unidadeId">ID da unidade</param>
        /// <param name="start">Data de início</param>
        /// <param name="end">Data de fim</param>
        /// <returns>Lista de escalas de trabalho</returns>
        [HttpGet("unidade/{unidadeId}")]
        public async Task<IActionResult> GetShifts(Guid unidadeId, [FromQuery] DateOnly start, [FromQuery] DateOnly end)
        {
            var result = await _service.GetShiftsAsync(unidadeId, start, end, GetTenantId());
            return Ok(result.Value);
        }

        [HttpPost("{shiftId}/breaks")]
        public async Task<IActionResult> AddBreak(Guid shiftId, [FromBody] AddBreakDto dto)
        {
            var result = await _service.AddBreakToShiftAsync(shiftId, dto, GetTenantId());
            if (!result.Success) return BadRequest(result.ErrorMessage);
            return Ok();
        }
    }
    #endregion

    #region DTO auxiliar para a requisição de geração
    // DTO auxiliar para a requisição de geração
    public class GenerateRequest
    {
        public Guid UnidadeId { get; set; }
        public Guid TemplateId { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public List<Guid> UserIds { get; set; } = new();
    }
    #endregion
}