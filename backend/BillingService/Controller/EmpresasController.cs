using BillingService.DTO;
using BillingService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BillingService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")] // Apenas Admins podem gerenciar empresas
public class EmpresasController : ControllerBase
{
    private readonly EmpresaService _empresaService;

    public EmpresasController(EmpresaService empresaService)
    {
        _empresaService = empresaService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllEmpresas()
    {
        var empresas = await _empresaService.GetAllEmpresasAsync();
        return Ok(empresas);
    }

    [HttpPost]
    public async Task<IActionResult> CreateEmpresa([FromBody] CreateEmpresaDto empresaDto)
    {
        var novaEmpresa = await _empresaService.CreateEmpresaAsync(empresaDto);
        return CreatedAtAction(nameof(GetAllEmpresas), new { id = novaEmpresa.Id }, novaEmpresa);
    }
}