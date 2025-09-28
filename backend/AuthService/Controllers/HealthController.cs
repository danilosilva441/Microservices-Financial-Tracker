using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

[ApiController]
public class HealthController : ControllerBase
{
    // Este endpoint é público e não requer autorização.
    [HttpGet("/health")]
    public IActionResult CheckHealth()
    {
        // Simplesmente retorna um status 200 OK para indicar que a API está no ar.
        return Ok("AuthService is Healthy");
    }
}