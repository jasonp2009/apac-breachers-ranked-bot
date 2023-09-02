using ApacBreachersRanked.Application.PingTimer.Models;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Infrastructure.Persistance
{
    internal partial class BreachersDbContext
    {
        public DbSet<TimedPing> TimedPings => Set<TimedPing>();

        partial void OnModelCreatingPingTimer(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TimedPing>(e =>
            {
                e.HasKey(p => p.RoleId);
            });
        }
    }
}
