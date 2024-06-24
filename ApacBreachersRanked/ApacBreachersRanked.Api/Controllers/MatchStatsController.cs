using ApacBreachersRanked.Api.Models.Stats;
using ApacBreachersRanked.Application.Stats.Queries;
using ApacBreachersRanked.Infrastructure.Breachers.Queries;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ApacBreachersRanked.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class MatchStatsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public MatchStatsController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpGet("MatchHistory")]
    [ProducesResponseType<IEnumerable<MatchDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMatchHistory(
        [FromQuery]GetMatchHistoryQuery query,
        CancellationToken cancellationToken)
    {
        var matches = await _mediator.Send(query, cancellationToken);
        return Ok(matches.Select(_mapper.Map<MatchDto>));
    }

    [HttpGet("UserMatchHistory")]
    [ProducesResponseType<IEnumerable<MatchDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserMatchHistory(
        [FromQuery]GetUserMatchHistoryQuery query,
        CancellationToken cancellationToken)
    {
        var matches = await _mediator.Send(query, cancellationToken);
        return Ok(matches.Select(_mapper.Map<MatchDto>));
    }

    [HttpGet("MatchData")]
    [ProducesResponseType<MatchDataDto>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMatchData(
        [FromQuery] GetMatchDataQuery query,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(query, cancellationToken);
        return Ok(_mapper.Map<MatchDataDto>(response));
    }
}
