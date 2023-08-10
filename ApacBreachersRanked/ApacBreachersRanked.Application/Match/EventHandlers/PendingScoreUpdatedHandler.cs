using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.Match.Extensions;
using ApacBreachersRanked.Application.Match.Models;
using Discord;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApacBreachersRanked.Application.Match.EventHandlers
{
    public class PendingScoreUpdatedHandler : INotificationHandler<PendingScoreUpdatedEvent>
    {
        private readonly IDiscordClient _discordClient;
        private readonly IDbContext _dbContext;
        private readonly ILogger<PendingScoreUpdatedHandler> _logger;

        public PendingScoreUpdatedHandler(IDiscordClient discordClient, IDbContext dbContext, ILogger<PendingScoreUpdatedHandler> logger)
        {
            _discordClient = discordClient;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Handle(PendingScoreUpdatedEvent notification, CancellationToken cancellationToken)
        {
            PendingMatchScore pendingMatchScore = await _dbContext.PendingMatchScores
                .SingleAsync(x => x.Id == notification.PendingMatchScoreId, cancellationToken);

            MatchThreads matchThreads = await _dbContext.MatchThreads.SingleAsync(x => x.Match.Id == pendingMatchScore.MatchId, cancellationToken);

            if (await _discordClient.GetChannelAsync(matchThreads.MatchThreadId) is IMessageChannel channel)
            {
                if (pendingMatchScore.MessageId == 0)
                {
                    ComponentBuilder cb = new();
                    cb.WithButton("Confirm", "pending-score-confirm", style: ButtonStyle.Success);
                    cb.WithButton("Reject", "pending-score-reject", style: ButtonStyle.Danger);

                    IUserMessage message = await channel.SendMessageAsync(embed: pendingMatchScore.GeneratePendingMatchScoreEmbed(), components: cb.Build());
                    pendingMatchScore.MessageId = message.Id;
                    await _dbContext.SaveChangesAsync(cancellationToken);
                    
                } else
                {
                    try
                    {
                        if (await channel.GetMessageAsync(pendingMatchScore.MessageId) is IUserMessage message)
                        {
                            await message.ModifyAsync(msg => msg.Embed = pendingMatchScore.GeneratePendingMatchScoreEmbed());
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex,
                            "An error occurred when trying to update the pending score message {PendingScoreMessageId} for pending match score {PendingMatchScoreId}. It may have been deleted.",
                            pendingMatchScore.MessageId,
                            notification.PendingMatchScoreId);
                    }
                    
                }
            }
        }
    }
}
