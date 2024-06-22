using ApacBreachersRanked.Application.Stats.Queries;
using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Infrastructure.Breachers.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ApacBreachersRanked.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class MatchStatsController : ControllerBase
{
    private readonly IMediator _mediator;

    public MatchStatsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("MatchHistory")]
    [ProducesResponseType<IEnumerable<MatchEntity>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMatchHistory(
        [FromQuery]GetMatchHistoryQuery query,
        CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(query, cancellationToken));
    }

    [HttpGet("UserMatchHistory")]
    [ProducesResponseType<IEnumerable<MatchEntity>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserMatchHistory(
        [FromQuery]GetUserMatchHistoryQuery query,
        CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(query, cancellationToken));
    }

    [HttpGet("MatchData")]
    [ProducesResponseType<GetMatchDataResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMatchData(
        [FromQuery] GetMatchDataQuery query,
        CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(query, cancellationToken));
    }
}
