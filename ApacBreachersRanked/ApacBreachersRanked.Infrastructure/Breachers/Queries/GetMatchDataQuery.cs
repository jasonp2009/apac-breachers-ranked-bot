using ApacBreachersRanked.Application.Common.Mediator;
using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Infrastructure.Breachers.Entities;
using ApacBreachersRanked.Infrastructure.Breachers.Services;

namespace ApacBreachersRanked.Infrastructure.Breachers.Queries;

public class GetMatchDataQuery : IQuery<GetMatchDataResponse>
{
    public Guid MatchId { get; set; }
}

public class GetMatchDataResponse
{
    public MatchEntity Match { get; set; }
    public MatchDataEntity MatchData { get; set; }
}

internal class GetMatchDataQueryHandler : IQueryHandler<GetMatchDataQuery, GetMatchDataResponse>
{
    private readonly MatchDataService _matchDataService;

    public GetMatchDataQueryHandler(MatchDataService matchDataService)
    {
        _matchDataService = matchDataService;
    }

    public async Task<GetMatchDataResponse> Handle(GetMatchDataQuery request, CancellationToken cancellationToken)
    {
        (MatchEntity match, MatchDataEntity matchData) =
            await _matchDataService.GetMatchData(request.MatchId, cancellationToken);
        return new()
        {
            Match = match,
            MatchData = matchData
        };
    }
}
