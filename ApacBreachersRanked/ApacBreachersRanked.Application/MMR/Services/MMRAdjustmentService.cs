using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.MMR.Entities;
using ApacBreachersRanked.Domain.MMR.Services;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Application.MMR.Services
{
    internal class MMRAdjustmentService : IMMRAdjustmentService
    {
        private readonly IDbContext _dbContext;

        public MMRAdjustmentService(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        async Task<List<PlayerMMR>> IMMRAdjustmentService.GetPlayerMMRsAsync(MatchEntity match, CancellationToken cancellationToken)
        {
            List<PlayerMMR> playerMMRs = new();

            foreach (MatchPlayer matchPlayer in match.AllPlayers)
            {
                PlayerMMR? playerMMR = await _dbContext.PlayerMMRs.FirstOrDefaultAsync(x => x.UserId.Equals(matchPlayer.UserId), cancellationToken);

                if (playerMMR == null)
                {
                    playerMMR = new(matchPlayer.UserId);
                    await _dbContext.PlayerMMRs.AddAsync(playerMMR, cancellationToken);
                }
                playerMMRs.Add(playerMMR);
            }

            return playerMMRs;
        }
    }
}
