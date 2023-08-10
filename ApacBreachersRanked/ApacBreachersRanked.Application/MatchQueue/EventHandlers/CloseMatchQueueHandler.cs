using ApacBreachersRanked.Application.Config;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.MatchQueue.Commands;
using ApacBreachersRanked.Application.MatchQueue.Models;
using ApacBreachersRanked.Domain.Match.Events;
using ApacBreachersRanked.Domain.MatchQueue.Entities;
using ApacBreachersRanked.Domain.MatchQueue.Events;
using ApacBreachersRanked.Domain.User.Interfaces;
using Discord;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Threading;

namespace ApacBreachersRanked.Application.MatchQueue.Events
{
    public class CloseMatchQueueHandler : INotificationHandler<MatchQueueClosedEvent>
    {
        private readonly IDbContext _dbContext;
        private readonly IDiscordClient _discordClient;
        private readonly IMediator _mediator;
        private readonly BreachersDiscordOptions _breachersDiscordOptions;

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
            MatchQueueEntity? matchQueue = await _dbContext.MatchQueue
                .Include(x => x.Match)
                .Include(x => x.Users)
                .FirstOrDefaultAsync(x => x.Id == notification.MatchQueueId, cancellationToken);

            if (matchQueue == null) return;

            CreateNewQueueWithRemainingPlayers(matchQueue);

            await _dbContext.SaveChangesAsync(cancellationToken);

            await DeleteOldQueueMessage(matchQueue, cancellationToken);
        }

        private async void CreateNewQueueWithRemainingPlayers(MatchQueueEntity matchQueue)
        {
            List<IUserId> matchPlayerIds = new();
            if (matchQueue.Match != null)
            {
                matchPlayerIds = matchQueue.Match.AllPlayers.Select(matchPlayer => matchPlayer.UserId).ToList();
            }

            List<MatchQueueUser> remainingUsers = matchQueue.Users.Where(matchQueueUser => !matchPlayerIds.Any(x => x.Equals(matchQueueUser.UserId))).ToList();

            MatchQueueEntity newMatchQueue = MatchQueueEntity.CreateNewQueueFromUsers(remainingUsers);

            await _dbContext.MatchQueue.AddAsync(newMatchQueue);
        }

        private async Task DeleteOldQueueMessage(MatchQueueEntity matchQueue, CancellationToken cancellationToken)
        {
            MatchQueueMessage? matchQueueMessage = await _dbContext.MatchQueueMessages.FirstOrDefaultAsync(x => x.MatchQueue == matchQueue, cancellationToken);
            if (matchQueueMessage == null || matchQueueMessage.IsDeleted) return;

            IMessageChannel channel = await _discordClient.GetChannelAsync(_breachersDiscordOptions.ReadyUpChannelId) as IMessageChannel;
            await channel.DeleteMessageAsync(matchQueueMessage.DiscordMessageId);
            matchQueueMessage.IsDeleted = true;
        }
    }
}
