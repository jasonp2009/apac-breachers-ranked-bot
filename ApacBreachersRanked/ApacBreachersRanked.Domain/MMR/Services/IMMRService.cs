using ApacBreachersRanked.Domain.Match.Enums;
using ApacBreachersRanked.Domain.MMR.Entities;
using ApacBreachersRanked.Domain.User.Interfaces;

namespace ApacBreachersRanked.Domain.MMR.Services
{
    public interface IMmrService
    {
        public Task<List<PlayerMMR>> GetPlayerMmRsAsync(IEnumerable<IUser> users, MatchFormat matchFormat, CancellationToken cancellationToken = default);
    }
}
