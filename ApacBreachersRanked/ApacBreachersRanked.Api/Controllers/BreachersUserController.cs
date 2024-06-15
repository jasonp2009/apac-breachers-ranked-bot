using ApacBreachersRanked.Api.Attributes;
using ApacBreachersRanked.Application.BreachersUsers.Commands;
using ApacBreachersRanked.Application.BreachersUsers.Models;
using ApacBreachersRanked.Application.BreachersUsers.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ApacBreachersRanked.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class BreachersUserController : ControllerBase
{
    private readonly IMediator _mediator;

    public BreachersUserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("SearchUsers")]
    [ProducesResponseType<IEnumerable<BreachersUser>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchUsers(
        [FromQuery] SearchBreachersUsersQuery query,
        CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(query, cancellationToken));
    }

    [RequireDiscordAuth]
    [HttpPost("LinkDiscordUser")]
    public async Task<IActionResult> LinkDiscordUser(
        [FromQuery] LinkDiscordUserCommand command,
        CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(command, cancellationToken));
    }

    [RequireDiscordAuth]
    [HttpGet("GetLinkedBreachersUser")]
    [ProducesResponseType<BreachersUser>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLinkedBreachersUser(
        [FromQuery] GetLinkedBreachersUserQuery query, CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(query, cancellationToken));
    }
}
