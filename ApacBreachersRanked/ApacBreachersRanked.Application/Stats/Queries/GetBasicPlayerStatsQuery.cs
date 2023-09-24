using ApacBreachersRanked.Application.Common.Mediator;
using ApacBreachersRanked.Application.DbContext;
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
    public class GetBasicPlayerStatsQuery : IQuery<BasicPlayerStats>
    {
        public ulong DiscordUserId { get; set; }
    }

    public class GetBasicStatsQueryHandler : IQueryHandler<GetBasicPlayerStatsQuery, BasicPlayerStats>
    {
        private readonly IDbContext _dbContext;
        private readonly IMediator _mediator;
        public GetBasicStatsQueryHandler(
            IDbContext dbContext,
            IMediator mediator)
        {
            _dbContext = dbContext;
            _mediator = mediator;
        }
        public async Task<BasicPlayerStats> Handle(GetBasicPlayerStatsQuery request, CancellationToken cancellationToken)
        {
            IUser user;
            try
            {
                user = await _mediator.Send(new GetDiscordUserQuery { DiscordUserId = request.DiscordUserId }, cancellationToken);
            }
            catch (Exception)
            {
                user = new UnknownDiscordUser(request.DiscordUserId.ToIUserId());
            }
            

            List<MatchEntity> matches = await _dbContext.Matches.AsNoTracking()
                .Where(x => x.Status == MatchStatus.Completed && x.AllPlayers.Any(player => player.UserId.Equals(user.UserId)))
                .ToListAsync(cancellationToken);

            PlayerMMR playerMMR = await _dbContext.PlayerMMRs
                .Where(x => x.UserId.Equals(user.UserId))
                .FirstOrDefaultAsync(cancellationToken)
                ?? new(user);

            BasicMatchPlayerStats matchStats = new();

            foreach (MatchEntity match in matches)
            {
                MatchPlayer matchPlayer = match.AllPlayers.Single(x => x.UserId.Equals(user.UserId));
                if (match.Score != null)
                {
                    matchStats.Played++;
                    if (match.Score.Outcome == ScoreOutcome.Draw)
                    {
                        matchStats.Drew++;
                    } else
                    {
                        if ((match.Score.Outcome == ScoreOutcome.Home && matchPlayer.Side == MatchSide.Home) ||
                            (match.Score.Outcome == ScoreOutcome.Away && matchPlayer.Side == MatchSide.Away))
                        {
                            matchStats.Won++;
                        } else
                        {
                            matchStats.Lost++;
                        }
                    }
                }
            }

            BasicPlayerStats stats = new()
            {
                User = user,
                MMR = playerMMR.MMR,
                Match = matchStats
            };
            return stats;
        }
    }
}
