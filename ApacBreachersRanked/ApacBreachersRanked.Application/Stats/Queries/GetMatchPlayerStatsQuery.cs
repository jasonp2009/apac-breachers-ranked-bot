using ApacBreachersRanked.Application.Common.Mediator;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.Stats.Enums;
using ApacBreachersRanked.Application.Stats.Models;
using ApacBreachersRanked.Application.Users;
using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.Match.Enums;
using ApacBreachersRanked.Domain.MMR.Entities;
using ApacBreachersRanked.Domain.User.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Application.Stats.Queries
{
    public class GetMatchPlayerStatsQuery : IQuery<MatchesPlayerStats>
    {
        public ulong DiscordUserId { get; set; }
    }

    public class GetMatchPlayerStatsQueryHandler : IQueryHandler<GetMatchPlayerStatsQuery, MatchesPlayerStats>
    {
        private readonly IDbContext _dbContext;
        private readonly IMediator _mediator;

        public GetMatchPlayerStatsQueryHandler(
            IDbContext dbContext,
            IMediator mediator)
        {
            _dbContext = dbContext;
            _mediator = mediator;
        }
        public async Task<MatchesPlayerStats> Handle(GetMatchPlayerStatsQuery request, CancellationToken cancellationToken)
        {
            IUser user = await _mediator.Send(new GetDiscordUserQuery { DiscordUserId = request.DiscordUserId }, cancellationToken);
            PlayerMMR? playerMMR = await _dbContext.PlayerMMRs
                .Include(x => x.Adjustments)
                .ThenInclude(x => x.Match)
                .FirstOrDefaultAsync(player => player.UserId.Equals(user.UserId), cancellationToken);

            MatchesPlayerStats stats = new MatchesPlayerStats
            {
                User = user,
                MMR = playerMMR?.MMR ?? default
            };

            if (playerMMR == null || playerMMR.Adjustments.Count == 0)
            {
                return stats;
            }

            foreach (MMRAdjustment adjustment in playerMMR.Adjustments.OrderByDescending(x => x.Match.MatchNumber).Take(5))
            {
                MatchPlayerStats matchStats = new()
                {
                    MatchNumber = adjustment.Match.MatchNumber,
                    MMRAdjustment = adjustment.Adjustment,
                };
                bool playerWasHome = adjustment.Match.HomePlayers.Any(player => player.UserId.Equals(user.UserId));

                MatchScore score = adjustment.Match.Score!;

                if (playerWasHome)
                {
                    matchStats.Outcome = score.Outcome == ScoreOutcome.Home ? MatchStatOutcome.Win
                        : score.Outcome == ScoreOutcome.Away ? MatchStatOutcome.Loss
                        : MatchStatOutcome.Draw;
                    matchStats.RoundsWon = score.RoundScore.Home;
                    matchStats.RoundsLost = score.RoundScore.Away;
                } else
                {
                    matchStats.Outcome = score.Outcome == ScoreOutcome.Away ? MatchStatOutcome.Win
                        : score.Outcome == ScoreOutcome.Home ? MatchStatOutcome.Loss
                        : MatchStatOutcome.Draw;
                    matchStats.RoundsWon = score.RoundScore.Away;
                    matchStats.RoundsLost = score.RoundScore.Home;
                }
                stats.Matches.Add(matchStats);
            }

            return stats;
        }
    }
}
