using ApacBreachersRanked.Application.BreachersUsers.Models;
using ApacBreachersRanked.Application.BreachersUsers.Services;
using ApacBreachersRanked.Application.Common.Mediator;
using ApacBreachersRanked.Application.Users;
using ApacBreachersRanked.Application.Users.Services;

namespace ApacBreachersRanked.Application.BreachersUsers.Queries;

public class GetLinkedBreachersUserQuery : IQuery<BreachersUser>
{
    public ulong? DiscordUserId { get; set; }
}

public class GetLinkedBreachersUserQueryHandler :
    IQueryHandler<GetLinkedBreachersUserQuery, BreachersUser>
{
    private readonly DiscordUserContextService _discordUserContextService;
    private readonly IBreachersUserService _breachersUserService;

    public GetLinkedBreachersUserQueryHandler(
        DiscordUserContextService discordUserContextService,
        IBreachersUserService breachersUserService)
    {
        _discordUserContextService = discordUserContextService;
        _breachersUserService = breachersUserService;
    }

    public Task<BreachersUser> Handle(GetLinkedBreachersUserQuery request, CancellationToken cancellationToken)
    {
        ulong discordUserId = request.DiscordUserId
                              ?? _discordUserContextService.GetDiscordUser()?.UserId.GetDiscordId()
                              ?? throw new ArgumentNullException(nameof(GetLinkedBreachersUserQuery.DiscordUserId));
        return _breachersUserService.GetBreachersUser(discordUserId, cancellationToken);
    }
}
