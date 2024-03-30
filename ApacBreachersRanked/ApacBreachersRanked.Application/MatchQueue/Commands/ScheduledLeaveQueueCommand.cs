using ApacBreachersRanked.Application.Common.Mediator;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.MatchQueue.Queries;
using ApacBreachersRanked.Application.Users;
using ApacBreachersRanked.Domain.MatchQueue.Entities;
using MediatR;

namespace ApacBreachersRanked.Application.MatchQueue.Commands
{
    public class ScheduledLeaveQueueCommand : ICommand
    {
        public ulong DiscordUserId { get; set; }
        public Guid ScheduledQueueId { get; set; }
    }
    
    public class ScheduledLeaveQueueCommandHandler : ICommandHandler<ScheduledLeaveQueueCommand>
    {
        private readonly IMediator _mediator;
        private readonly IDbContext _dbContext;

        public ScheduledLeaveQueueCommandHandler(IMediator mediator, IDbContext dbContext)
        {
            _mediator = mediator;
            _dbContext = dbContext;
        }
        public async Task<Unit> Handle(ScheduledLeaveQueueCommand request, CancellationToken cancellationToken)
        {
            ScheduledMatchQueueEntity scheduledQueue = await _mediator.Send(
                new GetScheduledQueueByIdQuery() { ScheduledMatchQueueId = request.ScheduledQueueId },
                cancellationToken);

            scheduledQueue.RemoveUserFromQueue(request.DiscordUserId.ToIUserId());

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
