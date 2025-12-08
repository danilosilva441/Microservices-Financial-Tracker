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

        [HttpPost("templates")]
        public async Task<IActionResult> CreateTemplate([FromBody] CreateWorkScheduleDto dto)
        {
            var result = await _service.CreateWorkScheduleAsync(dto, GetTenantId());
            if (!result.Success) return BadRequest(result.ErrorMessage);
            return Created("", result.Value);
        }

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

    // DTO auxiliar para a requisição de geração
    public class GenerateRequest
    {
        public Guid UnidadeId { get; set; }
        public Guid TemplateId { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public List<Guid> UserIds { get; set; } = new();
    }
}