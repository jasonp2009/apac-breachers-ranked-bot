using ApacBreachersRanked.Domain.Match.Entities;

namespace ApacBreachersRanked.Domain.Match.Services;

public interface IMatchDataService
{
    public Task<MatchScore> GetScore(Guid matchId, CancellationToken cancellationToken);
}
