using ApacBreachersRanked.Application.Common.Extensions;
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
    public class MapVoteCompletedHandler : INotificationHandler<MapVoteCompletedEvent>
    {
        private readonly IDbContext _dbContext;
        private readonly IDiscordClient _discordClient;
        private readonly ILogger<MapVoteCompletedHandler> _logger;

        public MapVoteCompletedHandler(
            IDbContext dbContext,
            IDiscordClient discordClient,
            ILogger<MapVoteCompletedHandler> logger)
        {
            _dbContext = dbContext;
            _discordClient = discordClient;
            _logger = logger;
        }

        public async Task Handle(MapVoteCompletedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                MatchVoteModel matchVote = await _dbContext.MatchVotes
                .Include(x => x.Match)
                .SingleAsync(x => x.MatchId == notification.MatchId, cancellationToken);

                MatchThreads matchThreads = await _dbContext.MatchThreads
                    .AsNoTracking()
                    .SingleAsync(x => x.Match.Id == notification.MatchId, cancellationToken);

                if (await _discordClient.GetChannelAsync(matchThreads.MatchThreadId) is IThreadChannel channel)
                {
                    if (await channel.GetMessageAsync(matchVote.HomeVoteMessageId!.Value) is IUserMessage homeVoteMessage)
                    {
                        await homeVoteMessage.ModifyAsync(msg =>
                        {
                            msg.Embed = matchVote.GetMapVoteEmbed();
                            msg.Components = new ComponentBuilder().Build();
                        });
                    }
                    var awayVoteMessage = await channel.SendMessageAsync(
                        text: string.Join(" ", matchVote.AwayVotes.Select(x => x.GetUserMention())),
                        embed: matchVote.GetSideVoteEmbed(),
                        components: matchVote.GetSideVoteComponents());

                    matchVote.AwayVoteMessageId = awayVoteMessage.Id;

                    await _dbContext.SaveChangesAsync(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred when trying to update messages on map vote completion for match {MatchId}", notification.MatchId);
            }
        }
    }
}
