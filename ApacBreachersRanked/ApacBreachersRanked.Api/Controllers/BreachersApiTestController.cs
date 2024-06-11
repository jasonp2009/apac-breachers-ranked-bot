using ApacBreachersRanked.Infrastructure.BreachersApi;
using Microsoft.AspNetCore.Mvc;

namespace ApacBreachersRanked.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class BreachersApiTestController : ControllerBase
{
    private BreachersApiClient _breachersApiClient;

    public BreachersApiTestController(BreachersApiClient breachersApiClient)
    {
        _breachersApiClient = breachersApiClient;
    }

    [HttpGet("SearchPlayers")]
    public async Task<IActionResult> SearchPlayers([FromQuery] string searchString)
    {
        return Ok(await _breachersApiClient.SearchUsers(searchString));
    }
}
