using MediatR;

namespace ApacBreachersRanked.Application.Users.Services;

public class DiscordUserContextService
{
    private ApplicationDiscordUser _discordUser;

    private IMediator _mediator;

    public DiscordUserContextService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task SetUserContext(ulong discordUserId, CancellationToken cancellationToken)
    {
        _discordUser =
            await _mediator.Send(new GetDiscordUserQuery { DiscordUserId = discordUserId }, cancellationToken) as
                ApplicationDiscordUser;
    }

    public ApplicationDiscordUser GetDiscordUser() => _discordUser;
}
