using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.Match.Models;
using ApacBreachersRanked.Application.MatchVote.Events;
using ApacBreachersRanked.Application.MatchVote.Extensions;
using ApacBreachersRanked.Application.MatchVote.Models;
using Discord;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApacBreachersRanked.Application.MatchVote.EventHandlers
{
    public class MapVoteUpdatedHandler : INotificationHandler<MapVoteUpdatedEvent>
    {
        private readonly IDbContext _dbContext;
        private readonly IDiscordClient _discordClient;
        private readonly ILogger<MapVoteUpdatedHandler> _logger;
        public MapVoteUpdatedHandler(
            IDbContext dbContext,
            IDiscordClient discordClient,
            ILogger<MapVoteUpdatedHandler> logger)
        {
            _dbContext = dbContext;
            _discordClient = discordClient;
            _logger = logger;
        }
        public async Task Handle(MapVoteUpdatedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                MatchVoteModel matchVote = await _dbContext.MatchVotes
                .AsNoTracking()
                .Include(x => x.Match)
                .Where(x => x.MatchId == notification.MatchId)
                .SingleAsync(cancellationToken);

                MatchThreads matchThreads = await _dbContext.MatchThreads
                    .AsNoTracking()
                    .Where(x => x.Match.Id == notification.MatchId)
                    .SingleAsync(cancellationToken);

                if (await _discordClient.GetChannelAsync(matchThreads.MatchThreadId) is IThreadChannel channel)
                {
                    if (await channel.GetMessageAsync(matchVote.HomeVoteMessageId!.Value) is IUserMessage message)
                    {
                        await message.ModifyAsync(msg =>
                        {
                            msg.Embed = matchVote.GetMapVoteEmbed();
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred when trying to updated the map vote message for match {MatchId}", notification.MatchId);
            }
        }
    }
}
