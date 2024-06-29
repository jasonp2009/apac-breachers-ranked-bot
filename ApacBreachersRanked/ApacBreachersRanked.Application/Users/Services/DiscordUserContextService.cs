using MediatR;
using Microsoft.Extensions.Logging;

namespace ApacBreachersRanked.Application.Users.Services;

public class DiscordUserContextService : IDisposable
{
    private ApplicationDiscordUser _discordUser;
    private IDisposable _logScope;

    private IMediator _mediator;
    private ILogger<DiscordUserContextService> _logger;

    public DiscordUserContextService(IMediator mediator,
        ILogger<DiscordUserContextService> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task SetUserContext(ulong discordUserId, CancellationToken cancellationToken = default)
    {
        _discordUser =
            await _mediator.Send(new GetDiscordUserQuery { DiscordUserId = discordUserId }, cancellationToken) as
                ApplicationDiscordUser;
        _logScope = _logger.BeginScope("With user context {DiscordUserId} {UserName}", discordUserId, _discordUser?.Name);
    }

    public void SetUserContext(Discord.IUser discordUser)
    {
        _discordUser = new(discordUser);;
        _logScope = _logger.BeginScope("With user context {DiscordUserId} {UserName}", discordUser.Id, _discordUser?.Name);
    }

    public ApplicationDiscordUser GetDiscordUser() => _discordUser;

    public void Dispose()
    {
        _logScope?.Dispose();
        _discordUser = null;
    }
}
