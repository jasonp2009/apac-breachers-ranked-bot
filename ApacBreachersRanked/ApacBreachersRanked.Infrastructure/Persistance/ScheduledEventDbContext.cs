using ApacBreachersRanked.Infrastructure.ScheduledEventHandling;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Infrastructure.Persistance
{
    internal partial class BreachersDbContext
    {
        internal DbSet<ScheduledEvent> ScheduledEvents => Set<ScheduledEvent>();

        partial void OnModelCreatingScheduledEvent(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ScheduledEvent>(e =>
            {
                e.Ignore(e => e.Event);
            });
        }
    }
}
