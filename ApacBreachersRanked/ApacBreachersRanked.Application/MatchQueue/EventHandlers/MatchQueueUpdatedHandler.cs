using ApacBreachersRanked.Application.Common.Extensions;
using ApacBreachersRanked.Application.Config;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.Match.Extensions;
using ApacBreachersRanked.Application.MatchQueue.Models;
using ApacBreachersRanked.Domain.Match.Enums;
using ApacBreachersRanked.Domain.MatchQueue.Entities;
using ApacBreachersRanked.Domain.MatchQueue.Events;
using Discord;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text;

namespace ApacBreachersRanked.Application.MatchQueue.Events
{
    public class MatchQueueUpdatedHandler : INotificationHandler<MatchQueueUpdatedEvent>
    {
        private readonly IDbContext _dbContext;
        private readonly IDiscordClient _discordClient;
        private readonly BreachersDiscordOptions _breachersDiscordOptions;
        public MatchQueueUpdatedHandler(
            IDbContext dbContext,
            IDiscordClient discordClient,
            IOptions<BreachersDiscordOptions> breachersDiscordOptions)
        {
            _dbContext = dbContext;
            _discordClient = discordClient;
            _breachersDiscordOptions = breachersDiscordOptions.Value;
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
            MatchQueueMessage? matchQueueMessage = matchQueueMessageTask.Result;
            IMessageChannel readyUpChannel = readyUpChannelTask.Result as IMessageChannel;


            if (matchQueueMessage != null)
            {
                IUserMessage message = await readyUpChannel.GetMessageAsync(matchQueueMessage.DiscordMessageId) as IUserMessage;
                await message.ModifyAsync(msg => msg.Embed = embed);
            }
            else
            {
                ComponentBuilder cb = new();
                cb.WithButton("Join", "match-queue-join", style: ButtonStyle.Success);
                cb.WithButton("Leave", "match-queue-leave", style: ButtonStyle.Danger);

                IUserMessage message = await readyUpChannel.SendMessageAsync(embed: embed, components: cb.Build());
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
            embedBuilder.WithDescription(string.Join(Environment.NewLine, users.Select(user => user.GetUserMention() + $" until {user.ExpiryUtc.ToDiscordRelativeEpoch()}")));
            StringBuilder footerBuilder = new();
            footerBuilder.AppendLine($"{users.Count}/10 players in queue");
            if (inProgressMatches != 0)
            {
                footerBuilder.AppendLine($"{inProgressMatches} match(s) in progress");
            }
            embedBuilder.WithFooter(footerBuilder.ToString());
            return embedBuilder.Build();
        }
    }
}
