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
    public class CreateMapVoteMessageHandler : INotificationHandler<MatchVoteCreatedEvent>
    {
        private readonly IDbContext _dbContext;
        private readonly IDiscordClient _discordClient;
        private readonly ILogger<CreateMapVoteMessageHandler> _logger;

        public CreateMapVoteMessageHandler(IDbContext dbContext, IDiscordClient discordClient, ILogger<CreateMapVoteMessageHandler> logger)
        {
            _dbContext = dbContext;
            _discordClient = discordClient;
            _logger = logger;
        }
        public async Task Handle(MatchVoteCreatedEvent notification, CancellationToken cancellationToken)
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
                try
                {
                    var mapVoteMessage = await channel.SendMessageAsync(
                        text: string.Join(" ", matchVote.HomeVotes.Select(x => x.GetUserMention())),
                        embed: matchVote.GetMapVoteEmbed(),
                        components: matchVote.GetMapVoteComponents());

                    matchVote.HomeVoteMessageId = mapVoteMessage.Id;
                    await _dbContext.SaveChangesAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An exception occurred when trying to create the map vote message for match: {MatchId}", notification.MatchId);
                }
            }
        }
    }
}
