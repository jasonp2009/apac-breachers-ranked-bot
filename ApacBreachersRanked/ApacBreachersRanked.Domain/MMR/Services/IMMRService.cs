using ApacBreachersRanked.Domain.Match.Enums;
using ApacBreachersRanked.Domain.MMR.Entities;
using ApacBreachersRanked.Domain.User.Interfaces;

namespace ApacBreachersRanked.Domain.MMR.Services
{
    public interface IMMRService
    {
        public Task<List<PlayerMMR>> GetPlayerMMRsAsync(IEnumerable<IUser> users, MatchFormat matchFormat, CancellationToken cancellationToken = default);
    }
}
