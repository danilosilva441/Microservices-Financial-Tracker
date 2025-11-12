using AuthService.DTO;
using AuthService.Services.Interfaces; // 1. IMPORTANTE: Usando o namespace das interfaces
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TenantController : ControllerBase
    {
        // 2. MUDANÇA: Injeta o ITenantService
        private readonly ITenantService _tenantService;

        public TenantController(ITenantService tenantService) // <-- MUDANÇA AQUI
        {
            _tenantService = tenantService;
        }

        [HttpPost("provision")]
        // [Authorize(Roles = "Dev")] 
        public async Task<IActionResult> ProvisionTenant([FromBody] TenantProvisionDto request)
        {
            // 3. MUDANÇA: Chama o _tenantService
            var result = await _tenantService.ProvisionTenantAsync(request);

            if (!result.Success)
            {
                // Se o usuário já existe
                if (result.ErrorMessage.Contains("já existe"))
                    return BadRequest(result.ErrorMessage);
                
                // Erro de configuração (ex: "Gerente" não encontrado)
                return StatusCode(500, result.ErrorMessage);
            }

            // Retorna 201 Created com os dados da nova conta
            return StatusCode(201, result.Data);
        }
    }
}