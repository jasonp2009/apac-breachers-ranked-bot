using ApacBreachersRanked.Application.BreachersUsers.Exceptions;
using ApacBreachersRanked.Application.BreachersUsers.Models;
using ApacBreachersRanked.Application.BreachersUsers.Services;
using ApacBreachersRanked.Application.Common.Mediator;
using ApacBreachersRanked.Application.Users;
using ApacBreachersRanked.Application.Users.Services;

namespace ApacBreachersRanked.Application.BreachersUsers.Queries;

public class IsUserLinkedQuery : IQuery<bool>
{
    public ulong? DiscordUserId { get; set; }
    public bool ThrowWhenNotLinked { get; set; } = false;
}

public class IsUserLinkedQueryHandler : IQueryHandler<IsUserLinkedQuery, bool>
{
    private readonly DiscordUserContextService _discordUserContextService;
    private readonly IBreachersUserService _breachersUserService;

    public IsUserLinkedQueryHandler(DiscordUserContextService discordUserContextService, IBreachersUserService breachersUserService)
    {
        _discordUserContextService = discordUserContextService;
        _breachersUserService = breachersUserService;
    }

    public async Task<bool> Handle(IsUserLinkedQuery request, CancellationToken cancellationToken)
    {
        ulong discordUserId = request.DiscordUserId
                              ?? _discordUserContextService.GetDiscordUser()?.UserId.GetDiscordId()
                              ?? throw new ArgumentNullException(nameof(GetLinkedBreachersUserQuery.DiscordUserId));
        BreachersUser linkedUser = await _breachersUserService.GetBreachersUser(discordUserId, cancellationToken);
        if (linkedUser == null && request.ThrowWhenNotLinked)
        {
            throw new UserNotLinkedException(discordUserId);
        }

        return linkedUser != null;
    }
}
