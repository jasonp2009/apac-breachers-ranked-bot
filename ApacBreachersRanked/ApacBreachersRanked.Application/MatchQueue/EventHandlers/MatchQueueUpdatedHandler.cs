using ApacBreachersRanked.Application.Common.Extensions;
using ApacBreachersRanked.Application.Config;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.Match.Extensions;
using ApacBreachersRanked.Application.MatchQueue.Models;
using ApacBreachersRanked.Domain.Match.Enums;
using ApacBreachersRanked.Domain.MatchQueue.Entities;
using ApacBreachersRanked.Domain.MatchQueue.Events;
using ApacBreachersRanked.Domain.User.Interfaces;
using Discord;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;

namespace ApacBreachersRanked.Application.MatchQueue.Events
{
    public class MatchQueueUpdatedHandler : INotificationHandler<MatchQueueUpdatedEvent>
    {
        private readonly IDbContext _dbContext;
        private readonly IDiscordClient _discordClient;
        private readonly BreachersDiscordOptions _breachersDiscordOptions;
        private readonly ILogger<MatchQueueUpdatedHandler> _logger;
        public MatchQueueUpdatedHandler(
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
        public async Task Handle(MatchQueueUpdatedEvent notification, CancellationToken cancellationToken)
        {
            MatchQueueEntity? matchQueue;
            if (notification.MatchQueueId != null)
            {
                matchQueue = await _dbContext.MatchQueue
                    .FirstOrDefaultAsync(x => x.Id == notification.MatchQueueId, cancellationToken);
            } else
            {
                matchQueue = await _dbContext.MatchQueue
                    .FirstOrDefaultAsync(x => x.IsOpen, cancellationToken);
            }
                
            if (matchQueue == null)
            {
                return;
            }
            int inProgressMatches = await _dbContext.Matches.CountAsync(match => match.Status == MatchStatus.PendingConfirmation || match.Status == MatchStatus.Confirmed, cancellationToken);
            Task<MatchQueueMessage?> matchQueueMessageTask = _dbContext.MatchQueueMessages.FirstOrDefaultAsync(x => x.MatchQueue.Id == matchQueue.Id, cancellationToken);
            Task<IChannel> readyUpChannelTask = _discordClient.GetChannelAsync(_breachersDiscordOptions.ReadyUpChannelId);

            await Task.WhenAll(
                matchQueueMessageTask,
                readyUpChannelTask);

            Embed embed = GetEmbed(matchQueue.Users, inProgressMatches);
            string pings = matchQueue.Users.Count >= 4 ? $"<@&{_breachersDiscordOptions.PingRoleId}>" : "";
            MatchQueueMessage? matchQueueMessage = matchQueueMessageTask.Result;
            IMessageChannel readyUpChannel = readyUpChannelTask.Result as IMessageChannel;

            if (matchQueueMessage?.DiscordMessageId != null && matchQueueMessage?.DiscordMessageId != 0)
            {
                try
                {
                    if (await readyUpChannel.GetMessageAsync(matchQueueMessage.DiscordMessageId) is IUserMessage message)
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
                cb.WithButton("Join 30", "join-queue-30", style: ButtonStyle.Success);
                cb.WithButton("Join 60", "join-queue-60", style: ButtonStyle.Success);
                cb.WithButton("Leave", "leave-queue", style: ButtonStyle.Danger);
                cb.WithButton("Force", "vote-force-match", style: ButtonStyle.Primary);

                IUserMessage message = await readyUpChannel.SendMessageAsync(text: pings, embed: embed, components: cb.Build());
                matchQueueMessage = new()
                {
                    MatchQueue = matchQueue,
                    DiscordMessageId = message.Id
                };
                _dbContext.MatchQueueMessages.Add(matchQueueMessage);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }

        private Embed GetEmbed(IList<MatchQueueUser> users, int inProgressMatches)
        {
            EmbedBuilder embedBuilder = new EmbedBuilder();
            embedBuilder.WithTitle("APAC Breachers Ranked Queue");
            embedBuilder.WithDescription(string.Join(Environment.NewLine, users.Select(GetUserLine)));
            StringBuilder footerBuilder = new();
            footerBuilder.AppendLine($"{users.Count}/10 players in queue");
            if (inProgressMatches != 0)
            {
                footerBuilder.AppendLine($"{inProgressMatches} match(s) in progress");
            }
            embedBuilder.WithFooter(footerBuilder.ToString());
            return embedBuilder.Build();
        }

        private static string ForceEmoji = "\uD83D\uDD2B";

        private string GetUserLine(MatchQueueUser user)
        {
            StringBuilder sb = new();
            if (user.VoteToForce) sb.Append($"{ForceEmoji} ");
            sb.Append(user.GetUserMention());
            sb.Append($" until {user.ExpiryUtc.ToDiscordRelativeEpoch()}");
            return sb.ToString();
        }
    }
}
