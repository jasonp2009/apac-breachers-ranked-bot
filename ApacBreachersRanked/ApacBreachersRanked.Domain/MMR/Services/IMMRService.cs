using ApacBreachersRanked.Domain.Match.Enums;
using ApacBreachersRanked.Domain.MMR.Entities;
using ApacBreachersRanked.Domain.User.Interfaces;

namespace ApacBreachersRanked.Domain.MMR.Services
{
    public interface IMmrService
    {
        public Task<List<PlayerMMR>> GetPlayerMmRsAsync(IEnumerable<IUser> users, MatchFormat matchFormat, bool includeAdjustments = false, CancellationToken cancellationToken = default);
        public Task<Dictionary<PlayerMMR, int>> GetMatchesPlayedAsync(IEnumerable<PlayerMMR> mmrs, CancellationToken cancellationToken = default);
    }
}
