using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.MatchQueue.Commands;
using ApacBreachersRanked.Application.Moderation.Events;
using ApacBreachersRanked.Application.Moderation.Models;
using ApacBreachersRanked.Application.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Application.Moderation.EventHandlers
{
    public class UnQueueBannedUserHandler : INotificationHandler<UserBannedEvent>
    {
        private readonly IMediator _mediator;
        private readonly IDbContext _dbContext;

        public UnQueueBannedUserHandler(IMediator mediator, IDbContext dbContext)
        {
            _mediator = mediator;
            _dbContext = dbContext;
        }

        public async Task Handle(UserBannedEvent notification, CancellationToken cancellationToken)
        {
            UserBan ban = await _dbContext.UserBans
                .AsNoTracking()
                .FirstAsync(x => x.Id == notification.UserBanId, cancellationToken);

            await _mediator.Send(new LeaveQueueCommand { DiscordUserId = ban.UserId.GetDiscordId() }, cancellationToken);
        }
    }
}
