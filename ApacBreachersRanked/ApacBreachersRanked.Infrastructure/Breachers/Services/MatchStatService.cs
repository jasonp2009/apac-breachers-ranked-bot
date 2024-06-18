using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.Match.Services;

namespace ApacBreachersRanked.Infrastructure.Breachers.Services;

public class MatchStatService : IMatchStatService
{
    private readonly IDbContext _dbContext;

    public MatchStatService(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<MatchScore> GetScore(Guid matchId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
