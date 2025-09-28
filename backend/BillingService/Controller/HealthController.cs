using Microsoft.AspNetCore.Mvc;

namespace BillingService.Controllers;

[ApiController]
public class HealthController : ControllerBase
{
    [HttpGet("/health")]
    public IActionResult CheckHealth()
    {
        return Ok("BillingService is Healthy");
    }
}