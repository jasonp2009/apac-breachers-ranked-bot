using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.Match.Models;
using Discord;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Application.Match.EventHandlers
{
    public class PendingScoreConfirmedHandler : INotificationHandler<PendingScoreConfirmedEvent>
    {
        private readonly IDbContext _dbContext;
        private readonly IDiscordClient _discordClient;
        
        public PendingScoreConfirmedHandler(IDbContext dbContext, IDiscordClient discordClient)
        {
            _dbContext = dbContext;
            _discordClient = discordClient;
        }
        public async Task Handle(PendingScoreConfirmedEvent notification, CancellationToken cancellationToken)
        {
            PendingMatchScore pendingMatchScore = await _dbContext.PendingMatchScores
                .Include(x => x.Match)
                .SingleAsync(x => x.Id == notification.PendingMatchScoreId, cancellationToken);

            IEnumerable<PendingMatchScore> otherPendingScores = _dbContext.PendingMatchScores
                .Where(x => x.MatchId == pendingMatchScore.MatchId && x.Id != pendingMatchScore.Id);

            MatchThreads matchThreads = await _dbContext.MatchThreads.SingleAsync(x => x.Match.Id == pendingMatchScore.MatchId, cancellationToken);

            if (await _discordClient.GetChannelAsync(matchThreads.MatchThreadId) is IThreadChannel channel)
            {
                List<Task> messageUpdateTasks = new();
                if (await channel.GetMessageAsync(pendingMatchScore.MessageId) is IUserMessage message)
                {
                    messageUpdateTasks.Add(message.ModifyAsync(msg => msg.Components = new ComponentBuilder().Build()));
                }
                foreach (PendingMatchScore otherPendingScore in otherPendingScores)
                {
                    if (await channel.GetMessageAsync(otherPendingScore.MessageId) is IUserMessage otherMessage)
                    {
                        messageUpdateTasks.Add(otherMessage.DeleteAsync());
                    }
                }
                await Task.WhenAll(messageUpdateTasks);
            }

            pendingMatchScore.Match.SetScore(pendingMatchScore.Score);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
