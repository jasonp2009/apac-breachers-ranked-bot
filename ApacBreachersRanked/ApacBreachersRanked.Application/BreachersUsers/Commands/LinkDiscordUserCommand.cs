using ApacBreachersRanked.Application.BreachersUsers.Services;
using ApacBreachersRanked.Application.Common.Mediator;
using ApacBreachersRanked.Application.Users.Services;
using MediatR;

namespace ApacBreachersRanked.Application.BreachersUsers.Commands;

public class LinkDiscordUserCommand : ICommand
{
    public string BreachersUserId { get; set; }
}

public class LinkDiscordUserCommandHandler : ICommandHandler<LinkDiscordUserCommand>
{
    private readonly DiscordUserContextService _discordUserContextService;
    private readonly IBreachersUserService _breachersUserService;

    public LinkDiscordUserCommandHandler(
        DiscordUserContextService discordUserContextService,
        IBreachersUserService breachersUserService)
    {
        _discordUserContextService = discordUserContextService;
        _breachersUserService = breachersUserService;
    }

    public async Task<Unit> Handle(LinkDiscordUserCommand request, CancellationToken cancellationToken)
    {
        var discordUser = _discordUserContextService.GetDiscordUser();
        await _breachersUserService.LinkDiscordUser(discordUser, request.BreachersUserId, cancellationToken);
        return Unit.Value;
    }
}
