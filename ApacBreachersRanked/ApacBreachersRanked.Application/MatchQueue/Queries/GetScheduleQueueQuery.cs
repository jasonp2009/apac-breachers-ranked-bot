using ApacBreachersRanked.Application.Common.Mediator;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Domain.Match.Constants;
using ApacBreachersRanked.Domain.Match.Enums;
using ApacBreachersRanked.Domain.MatchQueue.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Application.MatchQueue.Queries
{
    public class GetScheduleQueueQuery : IQuery<ScheduledMatchQueueEntity>
    {
        public DateTime ScheduledForUtc { get; set; }
        public bool CreateIfNotExists { get; set; } = true;
        public MatchFormat MatchFormat { get; set; } = MatchConstants.DefaultMatchFormat;
    }
    
    public class GetScheduleQueueQueryHandler : IQueryHandler<GetScheduleQueueQuery, ScheduledMatchQueueEntity>
    {
        private readonly IDbContext _dbContext;

        public GetScheduleQueueQueryHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ScheduledMatchQueueEntity> Handle(GetScheduleQueueQuery request, CancellationToken cancellationToken)
        {
            ScheduledMatchQueueEntity existingQueue = await _dbContext.ScheduleMatchQueues
                .Include(x => x.Users)
                .Where(x => x.IsOpen && x.ScheduledForUtc == request.ScheduledForUtc)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingQueue == null && request.CreateIfNotExists)
            {
                existingQueue = new(request.ScheduledForUtc, request.MatchFormat);
                await _dbContext.ScheduleMatchQueues.AddAsync(existingQueue, cancellationToken);
            }

            return existingQueue;
        }
    }
}
