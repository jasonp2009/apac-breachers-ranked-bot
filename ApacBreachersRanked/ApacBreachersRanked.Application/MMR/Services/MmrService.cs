using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.MMR.Queries;
using ApacBreachersRanked.Domain.Match.Enums;
using ApacBreachersRanked.Domain.MMR.Entities;
using ApacBreachersRanked.Domain.MMR.Services;
using ApacBreachersRanked.Domain.User.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Application.MMR.Services
{
    internal class MMRService : IMmrService
    {
        private readonly IMediator _mediator;
        private readonly IDbContext _dbContext;

        public MMRService(IMediator mediator, IDbContext dbContext)
        {
            _mediator = mediator;
            _dbContext = dbContext;
        }
        public Task<List<PlayerMMR>> GetPlayerMmRsAsync(IEnumerable<IUser> users, MatchFormat matchFormat, bool includeAdjustments = false, CancellationToken cancellationToken = default)
            => _mediator.Send(new GetPlayerMMRsQuery { Users = users, MatchFormat = matchFormat }, cancellationToken);

        public async Task<Dictionary<PlayerMMR, int>> GetMatchesPlayedAsync(IEnumerable<PlayerMMR> mmrs, CancellationToken cancellationToken = default)
        {
            Dictionary<PlayerMMR, int> matchesPlayed = new();

            foreach (var mmr in mmrs)
            {
                var count = await _dbContext.MMRAdjustments.CountAsync(
                    adj => adj.UserId.Equals(mmr.UserId) && adj.MatchFormat == mmr.MatchFormat, cancellationToken);
                matchesPlayed.Add(mmr, count);
            }

            return matchesPlayed;
        }
    }
}
