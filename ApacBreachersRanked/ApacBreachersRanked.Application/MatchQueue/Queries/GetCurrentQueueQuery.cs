using ApacBreachersRanked.Application.Common.Mediator;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Domain.Match.Enums;
using ApacBreachersRanked.Domain.MatchQueue.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Application.MatchQueue.Queries
{
    public class GetCurrentQueueQuery : IQuery<MatchQueueEntity>
    {
        public MatchFormat MatchFormat { get; init; }
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
            MatchQueueEntity currentQueue = await _dbContext.MatchQueue
                .Include(x => x.Users)
                .Where(x => x.IsOpen && x.MatchFormat == request.MatchFormat)
                .FirstOrDefaultAsync(cancellationToken);

            if (currentQueue == null)
            {
                currentQueue = new(request.MatchFormat);
                await _dbContext.MatchQueue.AddAsync(currentQueue);
            }

            return currentQueue;
        }
    }
}
