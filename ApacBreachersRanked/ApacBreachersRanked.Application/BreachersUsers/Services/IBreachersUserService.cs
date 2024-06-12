using ApacBreachersRanked.Application.BreachersUsers.Models;
using ApacBreachersRanked.Application.Users;

namespace ApacBreachersRanked.Application.BreachersUsers.Services;

public interface IBreachersUserService
{
    public Task<IEnumerable<BreachersUser>> SearchUsers(string searchString, CancellationToken cancellationToken);

    public Task LinkDiscordUser(ApplicationDiscordUser discordUser, string breachersUserId,
        CancellationToken cancellationToken);
    public Task<BreachersUser> GetBreachersUser(ApplicationDiscordUser discordUser, CancellationToken cancellationToken);
}
