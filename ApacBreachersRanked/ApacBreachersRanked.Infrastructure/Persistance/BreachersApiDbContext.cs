using ApacBreachersRanked.Infrastructure.Breachers.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Infrastructure.Persistance;

internal partial class BreachersDbContext 
{
    internal DbSet<BreachersDiscordUserLink> BreachersDiscordUserLinks => Set<BreachersDiscordUserLink>();

    partial void OnModelCreatingBreachersApi(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BreachersDiscordUserLink>(e =>
        {
            e.HasKey(p => p.DiscordUserId);
        });
    }
}
