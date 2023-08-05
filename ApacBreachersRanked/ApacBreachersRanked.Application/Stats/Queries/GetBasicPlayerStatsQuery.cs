﻿using ApacBreachersRanked.Application.Common.Mediator;
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
            IUser user = await _mediator.Send(new GetDiscordUserQuery { DiscordUserId = request.DiscordUserId }, cancellationToken);

            PlayerMMR playerMMR = await _dbContext.PlayerMMRs.FirstOrDefaultAsync(x => x.UserId.Equals(user.UserId), cancellationToken)
                ?? new(user);

            BasicMatchPlayerStats matchStats = new();

            foreach (MatchEntity match in _dbContext.Matches.Where(match => match.AllPlayers.Any(player => player.UserId.Equals(user.UserId)) && match.Status == MatchStatus.Completed))
            {
                if (match.Score != null)
                {
                    matchStats.Played++;
                    if (match.Score.Outcome == ScoreOutcome.Draw)
                    {
                        matchStats.Drew++;
                    } else
                    {
                        if (match.Score.Outcome == ScoreOutcome.Home &&
                            match.HomePlayers.Any(player => player.UserId.Equals(user.UserId)))
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