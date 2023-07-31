using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Domain.MatchQueue.Entities;
using ApacBreachersRanked.Domain.MatchQueue.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Application.MatchQueue.EventHandlers
{
    public class MatchQueueUserExpiredHandler : INotificationHandler<MatchQueueUserExpiredEvent>
    {
        private readonly IDbContext _dbContext;
        public MatchQueueUserExpiredHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task Handle(MatchQueueUserExpiredEvent notification, CancellationToken cancellationToken)
        {
            MatchQueueEntity? matchQueue = await _dbContext.MatchQueue.FirstOrDefaultAsync(x => x.IsOpen && x.Users.Any(user => user.UserId.Equals(notification.MatchQueueUserId)), cancellationToken);
            if (matchQueue == null) return;

            matchQueue.ExpireUser(notification.MatchQueueUserId);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
