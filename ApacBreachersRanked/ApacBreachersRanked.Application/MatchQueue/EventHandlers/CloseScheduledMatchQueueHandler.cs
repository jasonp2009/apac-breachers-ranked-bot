using ApacBreachersRanked.Application.Config;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.MatchQueue.Models;
using ApacBreachersRanked.Domain.MatchQueue.Entities;
using ApacBreachersRanked.Domain.MatchQueue.Events;
using Discord;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ApacBreachersRanked.Application.MatchQueue.EventHandlers
{
    public class CloseScheduledMatchQueueHandler : INotificationHandler<ScheduledMatchQueueClosedEvent>
    {
        private readonly IDbContext _dbContext;
        private readonly IDiscordClient _discordClient;
        private readonly BreachersDiscordOptions _breachersDiscordOptions;

        public CloseScheduledMatchQueueHandler(
            IDbContext dbContext,
            IDiscordClient discordClient,
            IOptions<BreachersDiscordOptions> breachersDiscordOptions)
        {
            _dbContext = dbContext;
            _discordClient = discordClient;
            _breachersDiscordOptions = breachersDiscordOptions.Value;
        }

        public async Task Handle(ScheduledMatchQueueClosedEvent notification, CancellationToken cancellationToken)
        {
            ScheduledMatchQueueEntity matchQueue = await _dbContext.ScheduleMatchQueues
                .Where(x => x.Id == notification.MatchQueueId)
                .FirstOrDefaultAsync(cancellationToken);
            if (matchQueue == null) return;
            
            ScheduledMatchQueueMessage matchQueueMessage = await _dbContext.ScheduledMatchQueueMessages
                .Where(x => x.MatchQueue == matchQueue)
                .FirstOrDefaultAsync(cancellationToken);
            
            if (matchQueueMessage == null || matchQueueMessage.IsDeleted) return;

            try
            {
                if (await _discordClient.GetChannelAsync(_breachersDiscordOptions.PlayingAtChannelId) is IMessageChannel
                    channel)
                {
                    await channel.DeleteMessageAsync(matchQueueMessage.DiscordMessageId);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
