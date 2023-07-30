using ApacBreachersRanked.Application.Config;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.Match.Extensions;
using ApacBreachersRanked.Application.MatchQueue.Models;
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
            MatchQueueEntity? matchQueue = await _dbContext.MatchQueue
                .FirstOrDefaultAsync(x => x.Id == notification.MatchQueueId, cancellationToken);
            if (matchQueue == null)
            {
                return;
            }
            Task<MatchQueueMessage?> matchQueueMessageTask = _dbContext.MatchQueueMessages.FirstOrDefaultAsync(x => x.MatchQueue.Id == notification.MatchQueueId, cancellationToken);
            Task<IChannel> readyUpChannelTask = _discordClient.GetChannelAsync(_breachersDiscordOptions.ReadyUpChannelId);

            await Task.WhenAll(
                matchQueueMessageTask,
                readyUpChannelTask);

            Embed embed = GetEmbed(matchQueue.Users);
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

        private Embed GetEmbed(IList<MatchQueueUser> users)
        {
            EmbedBuilder embedBuilder = new EmbedBuilder();
            embedBuilder.WithTitle("APAC Breachers Ranked Queue");
            StringBuilder sb = new();
            sb.AppendLine(string.Join(Environment.NewLine, users.Select(user => user.GetUserMention())));
            sb.AppendLine($"{users.Count}/10 players in queue");
            embedBuilder.WithDescription(sb.ToString());
            return embedBuilder.Build();
        }
    }
}
