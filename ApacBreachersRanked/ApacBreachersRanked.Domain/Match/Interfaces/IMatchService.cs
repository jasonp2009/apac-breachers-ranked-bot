using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.MatchQueue.Entities;

namespace ApacBreachersRanked.Domain.Match.Interfaces
{
    public interface IMatchService
    {
        public Task<MatchEntity> CreateMatchFromQueue(MatchQueueEntity matchQueue);
    }
}
