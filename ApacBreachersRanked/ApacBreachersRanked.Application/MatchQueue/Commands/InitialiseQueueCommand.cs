using ApacBreachersRanked.Application.Common.Mediator;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Domain.Helpers;
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
            var currentQueues = await _dbContext.MatchQueue
                .Where(x => x.IsOpen)
                .ToListAsync(cancellationToken);

            foreach (var matchFormat in MatchConstantsExtensions.GetEnabledMatchFormats())
            {
                var currentQueue = currentQueues.FirstOrDefault(queue => queue.MatchFormat == matchFormat);
                if (currentQueue is { IsOpen: true }) continue;
                currentQueue = new(matchFormat);
                currentQueue.QueueDomainEvent(new MatchQueueUpdatedEvent { MatchQueueId = currentQueue.Id });
                await _dbContext.MatchQueue.AddAsync(currentQueue);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            return Unit.Value;
        }
    }
}
