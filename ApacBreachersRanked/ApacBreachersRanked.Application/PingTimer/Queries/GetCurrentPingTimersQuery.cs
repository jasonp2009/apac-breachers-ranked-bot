using ApacBreachersRanked.Application.Common.Mediator;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.PingTimer.Models;

namespace ApacBreachersRanked.Application.PingTimer.Queries
{
    public class GetCurrentPingTimersQuery : IQuery<IEnumerable<TimedPing>>
    {
    }

    public class GetCurrentPingTimersHandler : IQueryHandler<GetCurrentPingTimersQuery, IEnumerable<TimedPing>>
    {
        private IDbContext _dbContext;

        public GetCurrentPingTimersHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public Task<IEnumerable<TimedPing>> Handle(GetCurrentPingTimersQuery request, CancellationToken cancellationToken)
            => Task.FromResult(_dbContext.TimedPings.AsEnumerable());
    }
}
