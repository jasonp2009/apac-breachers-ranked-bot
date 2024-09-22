using ApacBreachersRanked.Application.Config;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Domain.MatchQueue.Entities;
using ApacBreachersRanked.Domain.MatchQueue.Events;
using ApacBreachersRanked.Domain.User.Interfaces;
using Discord;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ApacBreachersRanked.Application.MatchQueue.Events;

public class CloseMatchQueueHandler : INotificationHandler<MatchQueueClosedEvent>
{
    private readonly BreachersDiscordOptions _breachersDiscordOptions;
    private readonly IDbContext _dbContext;
    private readonly IDiscordClient _discordClient;
    private readonly IMediator _mediator;

    public CloseMatchQueueHandler(
        IDbContext dbContext,
        IDiscordClient discordClient,
        IMediator mediator,
        IOptions<BreachersDiscordOptions> breachersDiscordOptions)
    {
        _dbContext = dbContext;
        _discordClient = discordClient;
        _mediator = mediator;
        _breachersDiscordOptions = breachersDiscordOptions.Value;
    }

    public async Task Handle(MatchQueueClosedEvent notification, CancellationToken cancellationToken)
    {
        if (await _dbContext.MatchQueue.AnyAsync(x => x.IsOpen, cancellationToken)) return;

        var matchQueue = await _dbContext.MatchQueue
            .Include(x => x.Match)
            .Include(x => x.Users)
            .Where(x => x.Id == notification.MatchQueueId)
            .FirstOrDefaultAsync(cancellationToken);

        if (matchQueue?.Match == null) return;

        await DeleteOldQueueMessage(matchQueue, cancellationToken);

        CreateNewQueueWithRemainingPlayers(matchQueue);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async void CreateNewQueueWithRemainingPlayers(MatchQueueEntity matchQueue)
    {
        List<IUserId> matchPlayerIds = new();
        if (matchQueue.Match != null)
            matchPlayerIds = matchQueue.Match.AllPlayers.Select(matchPlayer => matchPlayer.UserId).ToList();

        var remainingUsers = matchQueue.Users
            .Where(matchQueueUser => !matchPlayerIds.Any(x => x.Equals(matchQueueUser.UserId))).ToList();

        var newMatchQueue = MatchQueueEntity.CreateNewQueueFromUsers(remainingUsers, matchQueue.MatchFormat);

        await _dbContext.MatchQueue.AddAsync(newMatchQueue);
    }

    private async Task DeleteOldQueueMessage(MatchQueueEntity matchQueue, CancellationToken cancellationToken)
    {
        var matchQueueMessage = await _dbContext.MatchQueueMessages
            .Where(x => x.MatchQueue == matchQueue)
            .FirstOrDefaultAsync(cancellationToken);
        if (matchQueueMessage == null || matchQueueMessage.IsDeleted) return;

        var channel =
            await _discordClient.GetChannelAsync(_breachersDiscordOptions.ReadyUpChannelId) as IMessageChannel;
        await channel.DeleteMessageAsync(matchQueueMessage.DiscordMessageId);
        matchQueueMessage.IsDeleted = true;
    }
}