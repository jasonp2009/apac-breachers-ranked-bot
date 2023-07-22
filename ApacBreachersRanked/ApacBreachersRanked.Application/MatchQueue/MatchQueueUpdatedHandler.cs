using ApacBreachersRanked.Application.Config;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.Users;
using ApacBreachersRanked.Domain.Entities;
using Discord;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ApacBreachersRanked.Application.MatchQueue
{
    public class MatchQueueUpdatedHandler : INotificationHandler<MatchQueueUpdatedEvent>
    {
        private readonly IMatchQueueDbContext _dbContext;
        private readonly IDiscordClient _discordClient;
        private readonly IMediator _mediator;
        private readonly BreachersDiscordOptions _breachersDiscordOptions;
        public MatchQueueUpdatedHandler(
            IMatchQueueDbContext dbContext,
            IDiscordClient discordClient,
            IMediator mediator,
            IOptions<BreachersDiscordOptions> breachersDiscordOptions)
        {
            _dbContext = dbContext;
            _discordClient = discordClient;
            _mediator = mediator;
            _breachersDiscordOptions = breachersDiscordOptions.Value;
        }
        public async Task Handle(MatchQueueUpdatedEvent notification, CancellationToken cancellationToken)
        {
            MatchQueueEntity? matchQueue = await _dbContext.MatchQueue.FirstOrDefaultAsync(x => x.Id == notification.MatchQueueId, cancellationToken);
            if (matchQueue == null)
            {
                return;
            }
            IEnumerable<Task<ApplicationDiscordUser>> userTasks = matchQueue.Users.Select(user =>
            {
                return _mediator.Send(new GetDiscordUserQuery
                {
                    DiscordUserId = user.UserId as ApplicationDiscordUserId
                });
            });
            Task<MatchQueueMessage?> matchQueueMessageTask = _dbContext.MatchQueueMessages.FirstOrDefaultAsync(x => x.MatchQueueId == notification.MatchQueueId, cancellationToken);
            Task<IChannel> readyUpChannelTask = _discordClient.GetChannelAsync(_breachersDiscordOptions.ReadyUpChannelId);

            await Task.WhenAll(
                Task.WhenAll(userTasks),
                matchQueueMessageTask,
                readyUpChannelTask);

            IEnumerable<ApplicationDiscordUser> users = userTasks.Select(userTask => userTask.Result);
            Embed embed = GetEmbed(users);
            MatchQueueMessage? matchQueueMessage = matchQueueMessageTask.Result;
            IMessageChannel readyUpChannel = readyUpChannelTask.Result as IMessageChannel;


            if (matchQueueMessage != null)
            {
                IUserMessage message = await readyUpChannel.GetMessageAsync(matchQueueMessage.DiscordMessageId) as IUserMessage;
                await message.ModifyAsync(msg => msg.Embed = embed);
            } else
            {
                IUserMessage message = await readyUpChannel.SendMessageAsync(embed: embed);
                matchQueueMessage = new()
                {
                    MatchQueueId = matchQueue.Id,
                    DiscordMessageId = message.Id
                };
                _dbContext.MatchQueueMessages.Add(matchQueueMessage);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }

        private Embed GetEmbed(IEnumerable<ApplicationDiscordUser> users)
        {
            EmbedBuilder embedBuilder = new EmbedBuilder();
            embedBuilder.WithTitle("APAC Breachers Ranked Queue");
            embedBuilder.WithDescription(string.Join(Environment.NewLine, users.Select(user => user.Name)));
            return embedBuilder.Build();
        }


    }
}
