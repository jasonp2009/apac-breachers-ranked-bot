using ApacBreachersRanked.Application.Config;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.PingTimer.Events;
using ApacBreachersRanked.Application.PingTimer.Models;
using Discord;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ApacBreachersRanked.Application.PingTimer.EventHandlers
{
    public class RoleTimerExpiredHandler : INotificationHandler<RoleTimerExpiredEvent>
    {
        private readonly IDbContext _dbContext;
        private readonly IDiscordClient _discordClient;
        private readonly BreachersDiscordOptions _options;

        public RoleTimerExpiredHandler(
            IDbContext dbContext,
            IDiscordClient discordClient,
            IOptions<BreachersDiscordOptions> options)
        {
            _dbContext = dbContext;
            _discordClient = discordClient;
            _options = options.Value;
        }
        public async Task Handle(RoleTimerExpiredEvent notification, CancellationToken cancellationToken)
        {
            TimedPing? timedPing = await _dbContext.TimedPings
                .Where(x => x.RoleId == notification.RoleId)
                .FirstOrDefaultAsync(cancellationToken);
            if (timedPing == null) return;

            IGuild guild = await _discordClient.GetGuildAsync(_options.GuildId);

            IRole role = guild.GetRole(notification.RoleId);

            await role.ModifyAsync(rl => rl.Mentionable = true);

            timedPing.ReleaseTimeOut();

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
