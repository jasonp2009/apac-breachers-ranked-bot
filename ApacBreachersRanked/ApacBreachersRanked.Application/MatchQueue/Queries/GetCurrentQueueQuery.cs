using ApacBreachersRanked.Application.Common.Mediator;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Domain.MatchQueue.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Application.MatchQueue.Queries
{
    public class GetCurrentQueueQuery : IQuery<MatchQueueEntity>
    {
    }

    public class GetCurrentQueueHandler : IQueryHandler<GetCurrentQueueQuery, MatchQueueEntity>
    {
        private readonly IDbContext _dbContext;

        public GetCurrentQueueHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<MatchQueueEntity> Handle(GetCurrentQueueQuery request, CancellationToken cancellationToken)
        {
            MatchQueueEntity? currentQueue = await _dbContext.MatchQueue
                .Include(x => x.Users)
                .Where(x => x.IsOpen)
                .FirstOrDefaultAsync(cancellationToken);

            if (currentQueue == null)
            {
                currentQueue = new();
                await _dbContext.MatchQueue.AddAsync(currentQueue);
            }

            return currentQueue;
        }
    }
}
