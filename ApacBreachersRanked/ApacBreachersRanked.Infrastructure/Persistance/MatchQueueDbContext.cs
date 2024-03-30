using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.MatchQueue.Models;
using ApacBreachersRanked.Application.Users;
using ApacBreachersRanked.Domain.MatchQueue.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Infrastructure.Persistance
{
    internal partial class BreachersDbContext : IDbContext
    {

        public DbSet<MatchQueueEntity> MatchQueue => Set<MatchQueueEntity>();
        public DbSet<MatchQueueMessage> MatchQueueMessages => Set<MatchQueueMessage>();
        public DbSet<ScheduledMatchQueueEntity> ScheduleMatchQueues => Set<ScheduledMatchQueueEntity>();
        public DbSet<ScheduledMatchQueueMessage> ScheduledMatchQueueMessages => Set<ScheduledMatchQueueMessage>();

        partial void OnModelCreatingMatchQueue(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MatchQueueEntity>(e =>
            {
                e.UseTpcMappingStrategy().ToTable("MatchQueue");
                e.OwnsMany(x => x.Users, users =>
                {
                    users.Property(x => x.UserId).HasConversion(new ApplicationDiscordUserIdValueConvertor());
                });

                e.Navigation(x => x.Users);

                e.HasOne(x => x.Match)
                .WithOne()
                .IsRequired(false)
                .HasForeignKey<MatchQueueEntity>("MatchId");
            });

            modelBuilder.Entity<MatchQueueMessage>(e =>
            {
                e.HasOne(p => p.MatchQueue)
                .WithOne()
                .HasForeignKey<MatchQueueMessage>("MatchQueueId");
            });

            modelBuilder.Entity<ScheduledMatchQueueEntity>(e =>
            {
                e.UseTpcMappingStrategy().ToTable("ScheduledMatchQueues");
                e.OwnsMany(x => x.Users, users =>
                {
                    users.Property(x => x.UserId).HasConversion(new ApplicationDiscordUserIdValueConvertor());
                });

                e.Navigation(x => x.Users);
            });

            modelBuilder.Entity<ScheduledMatchQueueMessage>(e =>
            {
                e.ToTable("ScheduledMatchQueueMessages");
                e.HasOne(p => p.MatchQueue)
                    .WithOne()
                    .HasForeignKey<ScheduledMatchQueueMessage>("MatchQueueId");
            });
        }
    }
}
