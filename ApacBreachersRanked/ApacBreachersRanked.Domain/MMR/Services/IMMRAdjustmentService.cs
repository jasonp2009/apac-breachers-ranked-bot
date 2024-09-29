using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.MMR.Entities;

namespace ApacBreachersRanked.Domain.MMR.Services;

public interface IMmrAdjustmentService
{
    public IEnumerable<MMRAdjustment> CalculateAdjustments(MatchEntity match, IEnumerable<PlayerMMR> playerMmrs);
}
