using ApacBreachersRanked.Application.MatchVote.Models;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Application.DbContext
{
    public partial interface IDbContext
    {
        public DbSet<MatchVoteModel> MatchVotes { get; }
    }
}
