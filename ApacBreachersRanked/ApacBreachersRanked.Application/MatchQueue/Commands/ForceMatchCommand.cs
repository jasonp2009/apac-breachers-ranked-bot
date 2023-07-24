using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Domain.MatchQueue.Entities;
using ApacBreachersRanked.Domain.MatchQueue.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Application.MatchQueue.Commands
{
    public class ForceMatchCommand : IRequest<Unit>
    {
    }

    public class ForceMatchCommandHandler : IRequestHandler<ForceMatchCommand, Unit>
    {
        private readonly IDbContext _dbContext;

        public ForceMatchCommandHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Unit> Handle(ForceMatchCommand request, CancellationToken cancellationToken)
        {
            MatchQueueEntity? matchQueue = await _dbContext.MatchQueue.FirstOrDefaultAsync(x => x.IsOpen, cancellationToken);
            if (matchQueue == null) return Unit.Value;

            matchQueue.QueueDomainEvent(new MatchQueueCapacityReachedEvent() { MatchQueueId = matchQueue.Id });

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
