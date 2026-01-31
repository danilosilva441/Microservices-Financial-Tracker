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

        /// <summary>
        ///     Provisão de uma nova conta de inquilino (tenant) com usuário gerente
        /// </summary>
        /// <param name="request">Dados para provisão do tenant</param>
        /// <returns>Dados da nova conta ou mensagem de erro</returns>
        /// response code="201">Conta criada com sucesso</response>
        /// <response code="400">Dados inválidos ou usuário já existe</response>
        /// <response code="500">Erro interno do servidor relacionado à provisão</response>
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