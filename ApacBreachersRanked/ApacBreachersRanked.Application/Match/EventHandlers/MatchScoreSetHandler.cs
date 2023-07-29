using ApacBreachersRanked.Application.Config;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.Match.Extensions;
using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.Match.Events;
using Discord;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ApacBreachersRanked.Application.Match.EventHandlers
{
    public class MatchScoreSetHandler : INotificationHandler<MatchScoreSetEvent>
    {
        private readonly IDbContext _dbContext;
        private readonly IDiscordClient _discordClient;
        private readonly BreachersDiscordOptions _options;
        
        public MatchScoreSetHandler(IDbContext dbContext, IDiscordClient discordClient, IOptions<BreachersDiscordOptions> options)
        {
            _dbContext = dbContext;
            _discordClient = discordClient;
            _options = options.Value;
        }
        public async Task Handle(MatchScoreSetEvent notification, CancellationToken cancellationToken)
        {
            MatchEntity match = await _dbContext.Matches.AsNoTracking().SingleAsync(x => x.Id == notification.MatchId, cancellationToken);

            if (await _discordClient.GetChannelAsync(_options.MatchResultsChannelId) is IMessageChannel channel)
            {
                await channel.SendMessageAsync(embed: match.GenerateMatchResultEmbed());
            }
        }
    }
}
