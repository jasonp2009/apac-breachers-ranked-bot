using ApacBreachersRanked.Application.Moderation.Models;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Application.DbContext
{
    public partial interface IDbContext
    {
        public DbSet<UserBan> UserBans { get; }
        public DbSet<ActiveBansMessage> ActiveBansMessages { get; }
    }
}
