using ApacBreachersRanked.Application.MMR.Models;
using ApacBreachersRanked.Domain.MMR.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Application.DbContext
{
    public partial interface IDbContext
    {
        public DbSet<PlayerMMR> PlayerMMRs { get; }
        public DbSet<MMRAdjustment> MMRAdjustments { get; }
        public DbSet<LeaderBoardMessage> LeaderBoardMessages { get; }

        public Task ResetMMRAsync();
    }
}
