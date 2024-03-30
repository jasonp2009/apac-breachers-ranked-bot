using ApacBreachersRanked.Application.Common.Mediator;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Domain.MatchQueue.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Application.MatchQueue.Queries
{
    public class GetScheduledQueueByIdQuery : IQuery<ScheduledMatchQueueEntity>
    {
        public Guid ScheduledMatchQueueId { get; set; }
    }
    
    public class GetScheduledQueueByIdQueryHandler : IQueryHandler<GetScheduledQueueByIdQuery, ScheduledMatchQueueEntity>
    {
        private readonly IDbContext _dbContext;

        public GetScheduledQueueByIdQueryHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ScheduledMatchQueueEntity> Handle(GetScheduledQueueByIdQuery request, CancellationToken cancellationToken)
        {
            ScheduledMatchQueueEntity existingQueue = await _dbContext.ScheduleMatchQueues
                .Include(x => x.Users)
                .Where(x => x.Id == request.ScheduledMatchQueueId)
                .FirstOrDefaultAsync(cancellationToken);

            return existingQueue;
        }
    }
}
