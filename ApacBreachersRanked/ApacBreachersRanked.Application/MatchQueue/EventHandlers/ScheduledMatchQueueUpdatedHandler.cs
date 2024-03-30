using System.Text;
using ApacBreachersRanked.Application.Common.Extensions;
using ApacBreachersRanked.Application.Config;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.MatchQueue.Models;
using ApacBreachersRanked.Domain.MatchQueue.Entities;
using ApacBreachersRanked.Domain.MatchQueue.Events;
using Discord;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ApacBreachersRanked.Application.MatchQueue.EventHandlers
{
    public class ScheduledMatchQueueUpdatedHandler : INotificationHandler<ScheduledMatchQueueUpdatedEvent>
    {
        private readonly IDbContext _dbContext;
        private readonly IDiscordClient _discordClient;
        private readonly BreachersDiscordOptions _breachersDiscordOptions;
        private readonly ILogger<MatchQueueUpdatedHandler> _logger;
        
        public ScheduledMatchQueueUpdatedHandler(
            IDbContext dbContext,
            IDiscordClient discordClient,
            IOptions<BreachersDiscordOptions> breachersDiscordOptions,
            ILogger<MatchQueueUpdatedHandler> logger)
        {
            _dbContext = dbContext;
            _discordClient = discordClient;
            _breachersDiscordOptions = breachersDiscordOptions.Value;
            _logger = logger;
        }
        public async Task Handle(ScheduledMatchQueueUpdatedEvent notification, CancellationToken cancellationToken)
        {
            ScheduledMatchQueueEntity matchQueue = null;
            if (notification.MatchQueueId != null)
            {
                matchQueue = await _dbContext.ScheduleMatchQueues
                    .Include(x => x.Users)
                    .Where(x => x.Id == notification.MatchQueueId)
                    .FirstOrDefaultAsync(cancellationToken);
            }
                
            if (matchQueue == null)
            {
                return;
            }
            
            Task<ScheduledMatchQueueMessage> matchQueueMessageTask = _dbContext.ScheduledMatchQueueMessages
                .Where(x => x.MatchQueue.Id == matchQueue.Id)
                .FirstOrDefaultAsync(cancellationToken);
            Task<IChannel> playingAtChannelTask = _discordClient.GetChannelAsync(_breachersDiscordOptions.PlayingAtChannelId);

            await Task.WhenAll(
                matchQueueMessageTask,
                playingAtChannelTask);

            Embed embed = GetEmbed(matchQueue);
            string pings = $"<@&{_breachersDiscordOptions.PingRoleId}>";
            ScheduledMatchQueueMessage matchQueueMessage = matchQueueMessageTask.Result;
            IMessageChannel playingAtChannel = playingAtChannelTask.Result as IMessageChannel;

            if (matchQueueMessage?.DiscordMessageId != null && matchQueueMessage?.DiscordMessageId != 0)
            {
                try
                {
                    if (await playingAtChannel.GetMessageAsync(matchQueueMessage.DiscordMessageId) is IUserMessage message)
                    {
                        await message.ModifyAsync(msg =>
                        {
                            msg.Embed = embed;
                            msg.Content = pings;
                        });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "An exception occurred when trying to get the queue message, it may have been deleted");
                    return;
                }
            }
            else
            {
                ComponentBuilder cb = new();
                cb.WithButton("Join", $"scheduled-join-queue-{matchQueue.Id}", style: ButtonStyle.Success);
                cb.WithButton("Leave", $"scheduled-leave-queue-{matchQueue.Id}", style: ButtonStyle.Danger);

                IUserMessage message = await playingAtChannel.SendMessageAsync(text: pings, embed: embed, components: cb.Build());
                matchQueueMessage = new()
                {
                    MatchQueue = matchQueue,
                    DiscordMessageId = message.Id
                };
                _dbContext.ScheduledMatchQueueMessages.Add(matchQueueMessage);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }

        private Embed GetEmbed(ScheduledMatchQueueEntity matchQueue)
        {
            EmbedBuilder embedBuilder = new EmbedBuilder();
            embedBuilder.WithTitle($"Playing at {matchQueue.ScheduledForUtc.ToDiscordFullEpoch()}");
            embedBuilder.WithDescription(string.Join(Environment.NewLine, matchQueue.Users.Select(GetUserLine)));
            StringBuilder footerBuilder = new();
            footerBuilder.AppendLine($"{matchQueue.Users.Count}/10 players");
            embedBuilder.WithFooter(footerBuilder.ToString());
            return embedBuilder.Build();
        }

        private string GetUserLine(MatchQueueUser user)
        {
            StringBuilder sb = new();
            sb.Append(user.GetUserMention());
            return sb.ToString();
        }
    }
}
