using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Domain.Match.Events;
using Discord;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApacBreachersRanked.Application.Match.EventHandlers;

public class DeletePendingScoreMessages : INotificationHandler<MatchCompletedEvent>
{
    private readonly IDbContext _dbContext;
    private readonly IDiscordClient _discordClient;
    private readonly ILogger<DeletePendingScoreMessages> _logger;

    public DeletePendingScoreMessages(IDbContext dbContext, IDiscordClient discordClient,
        ILogger<DeletePendingScoreMessages> logger)
    {
        _dbContext = dbContext;
        _discordClient = discordClient;
        _logger = logger;
    }

    public async Task Handle(MatchCompletedEvent notification, CancellationToken cancellationToken)
    {
        var pendingScores = await _dbContext.PendingMatchScores
            .Where(x => x.MatchId == notification.MatchId)
            .ToListAsync(cancellationToken);

        if (!pendingScores.Any()) return;

        var matchThreads = await _dbContext.MatchThreads
            .Where(x => x.Match.Id == notification.MatchId)
            .SingleAsync(cancellationToken);

        if (await _discordClient.GetChannelAsync(matchThreads.MatchThreadId) is IThreadChannel channel)
            try
            {
                List<Task> messageUpdateTasks = new();
                foreach (var otherPendingScore in pendingScores)
                    if (await channel.GetMessageAsync(otherPendingScore.MessageId) is IUserMessage otherMessage)
                        messageUpdateTasks.Add(otherMessage.DeleteAsync());
                await Task.WhenAll(messageUpdateTasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "An exception occurred when trying to delete pending score messages for match {MatchId}",
                    notification.MatchId);
            }
    }
}