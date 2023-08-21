using ApacBreachersRanked.Application.Common.Mediator;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.MatchQueue.Queries;
using ApacBreachersRanked.Application.Users;
using ApacBreachersRanked.Domain.MatchQueue.Entities;
using MediatR;

namespace ApacBreachersRanked.Application.MatchQueue.Commands
{
    public class LeaveQueueCommand : ICommand
    {
        public ulong DiscordUserId { get; set; }
    }

    public class LeaveQueueCommandHandler : ICommandHandler<LeaveQueueCommand>
    {
        private readonly IMediator _mediator;
        private readonly IDbContext _dbContext;

        public LeaveQueueCommandHandler(IMediator mediator, IDbContext dbContext)
        {
            _mediator = mediator;
            _dbContext = dbContext;
        }
        public async Task<Unit> Handle(LeaveQueueCommand request, CancellationToken cancellationToken)
        {
            MatchQueueEntity currentQueue = await _mediator.Send(new GetCurrentQueueQuery(), cancellationToken);

            currentQueue.RemoveUserFromQueue(request.DiscordUserId.ToIUserId());

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
