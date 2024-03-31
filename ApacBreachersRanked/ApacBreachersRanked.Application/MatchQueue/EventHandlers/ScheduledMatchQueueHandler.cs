using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.MatchQueue.Queries;
using ApacBreachersRanked.Domain.MatchQueue.Entities;
using ApacBreachersRanked.Domain.MatchQueue.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Application.MatchQueue.EventHandlers
{
    public class ScheduledMatchQueueHandler : INotificationHandler<ScheduledMatchQueueEvent>
    {
        private readonly IDbContext _dbContext;
        private readonly IMediator _mediator;

        public ScheduledMatchQueueHandler(IDbContext dbContext, IMediator mediator)
        {
            _dbContext = dbContext;
            _mediator = mediator;
        }

        public async Task Handle(ScheduledMatchQueueEvent notification, CancellationToken cancellationToken)
        {
            ScheduledMatchQueueEntity schedulesMatchQueue = await _dbContext.ScheduleMatchQueues
                .FirstOrDefaultAsync(x => x.Id == notification.ScheduledMatchQueueId, cancellationToken);
            if (schedulesMatchQueue == null) return;

            schedulesMatchQueue.CloseQueue();

            var currentQueue = await _mediator.Send(new GetCurrentQueueQuery(), cancellationToken);

            foreach (var user in schedulesMatchQueue.Users)
            {
                if (await _mediator.Send(new IsUserInMatchQuery { UserId = user.UserId }, cancellationToken)) continue;
                
                currentQueue.AddUserToQueue(user, user.ExpiryUtc, user.JoinedAtUtc);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
