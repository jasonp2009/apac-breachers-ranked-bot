using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.MMR.Entities;

namespace ApacBreachersRanked.Domain.MMR.Services;

public interface IMmrAdjustmentService
{
    public Task<IEnumerable<MMRAdjustment>> CalculateAdjustmentsAsync(MatchEntity match, IEnumerable<PlayerMMR> playerMmrs, CancellationToken cancellationToken);
}
