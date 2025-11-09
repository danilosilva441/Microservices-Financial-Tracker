using AuthService.DTO;
using AuthService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TenantController : ControllerBase
    {
        private readonly IAuthService _authService;

        public TenantController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("provision")]
        // [Authorize(Roles = "Dev")] // TODO: Descomentar quando quiser proteger
        public async Task<IActionResult> ProvisionTenant([FromBody] TenantProvisionDto request)
        {
            var result = await _authService.ProvisionTenantAsync(request);

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