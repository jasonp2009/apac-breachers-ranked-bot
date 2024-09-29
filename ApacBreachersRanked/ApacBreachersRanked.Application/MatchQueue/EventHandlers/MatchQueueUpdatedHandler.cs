using System.Text;
using ApacBreachersRanked.Application.Common.Extensions;
using ApacBreachersRanked.Application.Config;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.MatchQueue.Models;
using ApacBreachersRanked.Domain.Helpers;
using ApacBreachersRanked.Domain.Match.Enums;
using ApacBreachersRanked.Domain.MatchQueue.Entities;
using ApacBreachersRanked.Domain.MatchQueue.Events;
using Discord;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ApacBreachersRanked.Application.MatchQueue.EventHandlers
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
            MatchQueueEntity matchQueue;
            if (notification.MatchQueueId != null)
            {
                matchQueue = await _dbContext.MatchQueue
                    .Include(x => x.Users)
                    .Where(x => x.Id == notification.MatchQueueId)
                    .FirstOrDefaultAsync(cancellationToken);
            } else
            {
                matchQueue = await _dbContext.MatchQueue
                    .Include(x => x.Users)
                    .Where(x => x.IsOpen)
                    .FirstOrDefaultAsync(cancellationToken);
            }
                
            if (matchQueue == null)
            {
                return;
            }
            int inProgressMatches = await _dbContext.Matches
                .Where(match => (match.Status == MatchStatus.PendingConfirmation || match.Status == MatchStatus.Confirmed) && match.MatchFormat == matchQueue.MatchFormat)
                .CountAsync(cancellationToken);
            Task<MatchQueueMessage?> matchQueueMessageTask = _dbContext.MatchQueueMessages
                .Where(x => x.MatchQueue.Id == matchQueue.Id)
                .FirstOrDefaultAsync(cancellationToken);
            Task<IChannel> readyUpChannelTask = _discordClient.GetChannelAsync(_breachersDiscordOptions.ReadyUpChannelId);

            await Task.WhenAll(
                matchQueueMessageTask,
                readyUpChannelTask);

            Embed embed = GetEmbed(matchQueue, inProgressMatches);
            string pings = matchQueue.Users.Count >= matchQueue.MatchFormat.GetMatchConstant(c => c.PingAtPlayers)
                ? $"<@&{_breachersDiscordOptions.PingRoleId}>"
                : string.Empty;
            MatchQueueMessage? matchQueueMessage = matchQueueMessageTask.Result;
            
            IMessageChannel readyUpChannel = readyUpChannelTask.Result as IMessageChannel;
            if (readyUpChannel is null) throw new NullReferenceException("Could not find ready up channel");

            if (pings != string.Empty &&
                matchQueueMessage is not null &&
                matchQueueMessage.DiscordMessageId != 0 &&
                matchQueueMessage.IsDeleted == false &&
                (matchQueueMessage.LastPingedUtc is null ||
                matchQueueMessage.LastPingedUtc + TimeSpan.FromMinutes(30) < DateTime.UtcNow))
            {
                await readyUpChannel.DeleteMessageAsync(matchQueueMessage.DiscordMessageId);
                matchQueueMessage.IsDeleted = true;
                await _dbContext.SaveChangesAsync(cancellationToken);
                matchQueueMessage = null;
            }
            
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
                cb.WithButton("Join 30", $"join-queue-30-{matchQueue.MatchFormat}", style: ButtonStyle.Success);
                cb.WithButton("Join 60", $"join-queue-60-{matchQueue.MatchFormat}", style: ButtonStyle.Success);
                cb.WithButton("Leave", "leave-queue", style: ButtonStyle.Danger);
                cb.WithButton("Force", $"vote-force-match-{matchQueue.MatchFormat}", style: ButtonStyle.Primary);

                IUserMessage message = await readyUpChannel.SendMessageAsync(text: pings, embed: embed, components: cb.Build());
                matchQueueMessage = new()
                {
                    MatchQueue = matchQueue,
                    DiscordMessageId = message.Id,
                    LastPingedUtc = pings != string.Empty ? DateTime.UtcNow : null
                };
                _dbContext.MatchQueueMessages.Add(matchQueueMessage);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }

        private Embed GetEmbed(MatchQueueEntity matchQueue, int inProgressMatches)
        {
            var users = matchQueue.Users;
            EmbedBuilder embedBuilder = new EmbedBuilder();
            embedBuilder.WithTitle($"APAC Breachers {matchQueue.MatchFormat.GetFriendlyName()} Queue");
            embedBuilder.WithDescription(string.Join(Environment.NewLine, users.Select(GetUserLine)));
            StringBuilder footerBuilder = new();
            footerBuilder.AppendLine($"{users.Count}/{matchQueue.MatchFormat.GetMatchConstant(c => c.MaxCapacity)} players in queue");
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
