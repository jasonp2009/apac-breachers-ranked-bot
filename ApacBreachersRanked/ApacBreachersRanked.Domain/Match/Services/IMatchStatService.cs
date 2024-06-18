using ApacBreachersRanked.Domain.Match.Entities;

namespace ApacBreachersRanked.Domain.Match.Services;

public interface IMatchStatService
{
    public Task<MatchScore> GetScore(Guid matchId, CancellationToken cancellationToken);
}
