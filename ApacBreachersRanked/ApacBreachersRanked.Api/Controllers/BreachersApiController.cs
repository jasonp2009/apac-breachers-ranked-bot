using ApacBreachersRanked.Infrastructure.Breachers.Api;
using ApacBreachersRanked.Infrastructure.Breachers.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApacBreachersRanked.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class BreachersApiController : ControllerBase
{
    private readonly BreachersApiClient _breachersApiClient;

    public BreachersApiController(BreachersApiClient breachersApiClient)
    {
        _breachersApiClient = breachersApiClient;
    }
    
    [HttpGet("get_match_data")]
    [ProducesResponseType<IEnumerable<GetMatchResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMatchData(
        [FromQuery] string player_id,
        CancellationToken cancellationToken)
    {
        IEnumerable<GetMatchResponse> response = await _breachersApiClient.GetMatchesByUserId(player_id, cancellationToken);
        return Ok(response);
    }
}
