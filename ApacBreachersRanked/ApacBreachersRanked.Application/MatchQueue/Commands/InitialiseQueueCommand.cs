using ApacBreachersRanked.Application.Common.Mediator;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Domain.MatchQueue.Entities;
using ApacBreachersRanked.Domain.MatchQueue.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Application.MatchQueue.Commands
{
    public class InitialiseQueueCommand : ICommand
    {
    }

    public class InitialiseQueueCommandHandler : ICommandHandler<InitialiseQueueCommand>
    {
        private readonly IDbContext _dbContext;
        public InitialiseQueueCommandHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Unit> Handle(InitialiseQueueCommand request, CancellationToken cancellationToken)
        {
            MatchQueueEntity? currentQueue = await _dbContext.MatchQueue.FirstOrDefaultAsync(x => x.IsOpen, cancellationToken);

            if (currentQueue == null || !currentQueue.IsOpen)
            {
                currentQueue = new();
                currentQueue.QueueDomainEvent(new MatchQueueUpdatedEvent { MatchQueueId = currentQueue.Id });
                await _dbContext.MatchQueue.AddAsync(currentQueue);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            return Unit.Value;
        }
    }
}
