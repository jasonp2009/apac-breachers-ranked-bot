using ApacBreachersRanked.Application.Common.Mediator;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.MatchData.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Application.Match.Queries;

public class GetMatchDataQuery : IQuery<GetMatchDataResponse>
{
    public Guid MatchId { get; set; }
}

public class GetMatchDataResponse
{
    public MatchEntity Match { get; set; }
    public IEnumerable<GameDataEntity> GameData { get; set; }
}

public class GetMatchDataHandler : IQueryHandler<GetMatchDataQuery, GetMatchDataResponse>
{
    private readonly IDbContext _dbContext;

    public GetMatchDataHandler(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetMatchDataResponse> Handle(GetMatchDataQuery request, CancellationToken cancellationToken)
    {
        MatchEntity match = await _dbContext.Matches
            .Include(match => match.AllPlayers)
            .FirstOrDefaultAsync(match => match.Id == request.MatchId,
            cancellationToken);
        List<GameDataEntity> gameData = await _dbContext.GameData
            .Where(game => game.MatchId == request.MatchId)
            .ToListAsync(cancellationToken);
        return new()
        {
            Match = match,
            GameData = gameData
        };
    }
}
