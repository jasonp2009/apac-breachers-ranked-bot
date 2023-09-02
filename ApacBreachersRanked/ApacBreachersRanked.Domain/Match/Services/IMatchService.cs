using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.MatchQueue.Entities;

namespace ApacBreachersRanked.Domain.Match.Services
{
    public interface IMatchService
    {
        public Task<MatchEntity> CreateMatchFromQueueAsync(MatchQueueEntity matchQueue, CancellationToken cancellationToken);
    }
}
