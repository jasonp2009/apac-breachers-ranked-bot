using Microsoft.AspNetCore.Mvc;

namespace ApacBreachersRanked.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthCheckController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> HealthCheck()
    {
        return Ok("Healthy");
    }
}
