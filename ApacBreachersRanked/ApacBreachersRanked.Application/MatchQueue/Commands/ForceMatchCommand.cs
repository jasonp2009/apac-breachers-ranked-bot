using ApacBreachersRanked.Application.Common.Mediator;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Domain.MatchQueue.Entities;
using ApacBreachersRanked.Domain.MatchQueue.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Application.MatchQueue.Commands
{
    public class ForceMatchCommand : ICommand
    {
    }

    public class ForceMatchCommandHandler : ICommandHandler<ForceMatchCommand>
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

            if (matchQueue.Users.Count < 6) throw new InvalidOperationException("There must be atleast 6 players to force match");

            matchQueue.QueueDomainEvent(new MatchQueueCapacityReachedEvent() { MatchQueueId = matchQueue.Id });

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
