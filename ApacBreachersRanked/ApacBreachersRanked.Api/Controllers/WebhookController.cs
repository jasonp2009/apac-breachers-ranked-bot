using Microsoft.AspNetCore.Mvc;

namespace ApacBreachersRanked.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class WebhookController : ControllerBase
{
    private readonly ILogger<WebhookController> _logger;

    public WebhookController(ILogger<WebhookController> logger)
    {
        _logger = logger;
    }

    [HttpPost("DeleteUser")]
    public async Task<IActionResult> DeleteUserAsync(object body)
    {
        _logger.LogInformation("Received Delete User request with {@RequestBody}", body);
        return Ok();
    }
}
