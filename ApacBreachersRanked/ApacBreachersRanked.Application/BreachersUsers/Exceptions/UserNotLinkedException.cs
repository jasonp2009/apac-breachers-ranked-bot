namespace ApacBreachersRanked.Application.BreachersUsers.Exceptions;

public class UserNotLinkedException : Exception
{
    private ulong DiscordUserId { get; set; }

    public UserNotLinkedException(ulong discordUserId)
    {
        DiscordUserId = discordUserId;
    }
}
