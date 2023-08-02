using ApacBreachersRanked.Domain.MMR.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Application.DbContext
{
    public partial interface IDbContext
    {
        public DbSet<PlayerMMR> PlayerMMRs { get; }
        public DbSet<MMRAdjustment> MMRAdjustments { get; }

        public Task ResetMMRAsync();
    }
}
