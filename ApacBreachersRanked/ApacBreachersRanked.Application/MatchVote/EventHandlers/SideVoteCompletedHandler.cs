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
    public class SideVoteCompletedHandler : INotificationHandler<SideVoteCompletedEvent>
    {
        private readonly IDbContext _dbContext;
        private readonly IDiscordClient _discordClient;
        private readonly ILogger<MapVoteCompletedHandler> _logger;

        public SideVoteCompletedHandler(
            IDbContext dbContext,
            IDiscordClient discordClient,
            ILogger<MapVoteCompletedHandler> logger)
        {
            _dbContext = dbContext;
            _discordClient = discordClient;
            _logger = logger;
        }

        public async Task Handle(SideVoteCompletedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                MatchVoteModel matchVote = await _dbContext.MatchVotes
                .Include(x => x.Match)
                .Where(x => x.MatchId == notification.MatchId)
                .SingleAsync(cancellationToken);

                MatchThreads matchThreads = await _dbContext.MatchThreads
                    .AsNoTracking()
                    .Where(x => x.Match.Id == notification.MatchId)
                    .SingleAsync(cancellationToken);

                if (await _discordClient.GetChannelAsync(matchThreads.MatchThreadId) is IThreadChannel channel)
                {
                    if (await channel.GetMessageAsync(matchVote.AwayVoteMessageId!.Value) is IUserMessage awayVoteMessage)
                    {
                        await awayVoteMessage.ModifyAsync(msg =>
                        {
                            msg.Embed = matchVote.GetSideVoteEmbed();
                            msg.Components = new ComponentBuilder().Build();
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred when trying to update messages on map vote completion for match {MatchId}", notification.MatchId);
            }
        }
    }
}
