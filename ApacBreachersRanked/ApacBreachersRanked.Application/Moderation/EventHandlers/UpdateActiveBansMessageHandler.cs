using ApacBreachersRanked.Application.Common.Extensions;
using ApacBreachersRanked.Application.Common.Mediator;
using ApacBreachersRanked.Application.Config;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.Moderation.Commands;
using ApacBreachersRanked.Application.Moderation.Events;
using ApacBreachersRanked.Application.Moderation.Extensions;
using ApacBreachersRanked.Application.Moderation.Models;
using ApacBreachersRanked.Application.Moderation.Queries;
using Discord;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ApacBreachersRanked.Application.Moderation.EventHandlers
{
    public class UpdateActiveBansMessageHandler :
        INotificationHandler<UserBannedEvent>,
        INotificationHandler<UserBanExpiredEvent>,
        INotificationHandler<UserUnBannedEvent>,
        ICommandHandler<RefreshBanMessageCommand>
    {
        private readonly IMediator _mediator;
        private readonly IDbContext _dbContext;
        private readonly IDiscordClient _discordClient;
        private readonly BreachersDiscordOptions _options;

        public UpdateActiveBansMessageHandler(
            IMediator mediator,
            IDbContext dbContext,
            IDiscordClient discordClient,
            IOptions<BreachersDiscordOptions> options)
        {
            _mediator = mediator;
            _dbContext = dbContext;
            _discordClient = discordClient;
            _options = options.Value;
        }

        public Task Handle(UserBannedEvent notification, CancellationToken cancellationToken)
            => InnerHandle(cancellationToken);

        public Task Handle(UserBanExpiredEvent notification, CancellationToken cancellationToken)
            => InnerHandle(cancellationToken);

        public Task Handle(UserUnBannedEvent notification, CancellationToken cancellationToken)
            => InnerHandle(cancellationToken);

        public async Task<Unit> Handle(RefreshBanMessageCommand request, CancellationToken cancellationToken)
        {
            await InnerHandle(cancellationToken);
            return Unit.Value;
        }

        private async Task InnerHandle(CancellationToken cancellationToken)
        {
            IQueryable<UserBan> activeBans = await _mediator.Send(new GetActiveBansQuery(), cancellationToken);
            ActiveBansMessage? activeBanMessage = await _dbContext.ActiveBansMessages.FirstOrDefaultAsync(cancellationToken);

            if (activeBanMessage == null)
            {
                activeBanMessage = new();
                await _dbContext.ActiveBansMessages.AddAsync(activeBanMessage);
            }

            if (await _discordClient.GetChannelAsync(_options.ActiveBanChannelId) is ITextChannel channel)
            {
                if (activeBanMessage.ActiveBansMessageId != 0 &&
                    await channel.GetMessageAsync(activeBanMessage.ActiveBansMessageId) is IUserMessage message)
                {
                    await message.ModifyAsync(msg =>
                    {
                        msg.Content = string.Join(" ", activeBans.Select(x => x.GetUserMention()));
                        msg.Embed = activeBans.GetActiveBansEmbed();
                    });
                }
                else
                {
                    IUserMessage newMessage = await channel.SendMessageAsync(string.Join(" ", activeBans.Select(x => x.GetUserMention())), embed: activeBans.GetActiveBansEmbed());
                    activeBanMessage.ActiveBansMessageId = newMessage.Id;
                    await _dbContext.SaveChangesAsync(cancellationToken);
                }
            }
        } 
    }
}
