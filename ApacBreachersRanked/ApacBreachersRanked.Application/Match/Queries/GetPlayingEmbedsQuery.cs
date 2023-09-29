using ApacBreachersRanked.Application.Common.Mediator;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.Match.Extensions;
using ApacBreachersRanked.Application.MatchVote.Models;
using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.Match.Enums;
using Discord;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Application.Match.Queries
{
    public class GetPlayingEmbedsQuery : IQuery<List<Embed>>
    {
    }

    public class GetPlayingEmbedsHandler : IQueryHandler<GetPlayingEmbedsQuery, List<Embed>>
    {
        private readonly IDbContext _dbContext;

        public GetPlayingEmbedsHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<Embed>> Handle(GetPlayingEmbedsQuery request, CancellationToken cancellationToken)
        {
            List<MatchEntity> matches = await _dbContext.Matches
                .Include(x => x.AllPlayers)
                .Include(x => x.Score)
                .Where(x => x.Status == MatchStatus.PendingConfirmation || x.Status == MatchStatus.Confirmed).ToListAsync(cancellationToken);
            List<Embed> embeds = new();

            foreach (MatchEntity match in matches)
            {
                MatchVoteModel? matchVote = await _dbContext.MatchVotes
                    .Where(x => x.MatchId == match.Id)
                    .FirstOrDefaultAsync(cancellationToken);
                embeds.Add(match.GenerateCurrentMatchEmbed(matchVote));
            }

            return embeds;
        }
    }
}
