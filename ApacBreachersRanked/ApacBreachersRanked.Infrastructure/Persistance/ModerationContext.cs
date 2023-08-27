using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.Moderation.Models;
using ApacBreachersRanked.Application.Users;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Infrastructure.Persistance
{
    internal partial class BreachersDbContext : IDbContext
    {
        public DbSet<UserBan> UserBans => Set<UserBan>();
        public DbSet<ActiveBansMessage> ActiveBansMessages => Set<ActiveBansMessage>();

        partial void OnModelCreatingModeration(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserBan>(e =>
            {
                e.Property(p => p.UserId).HasConversion(new ApplicationDiscordUserIdValueConvertor());
            });
        }
    }
}
