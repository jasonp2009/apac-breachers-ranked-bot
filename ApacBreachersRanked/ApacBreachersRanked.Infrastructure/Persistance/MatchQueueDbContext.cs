using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.MatchQueue.Models;
using ApacBreachersRanked.Application.Users;
using ApacBreachersRanked.Domain.MatchQueue.Entities;
using ApacBreachersRanked.Infrastructure.Config;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace ApacBreachersRanked.Infrastructure.Persistance
{
    internal partial class BreachersDbContext : IDbContext
    {

        public DbSet<MatchQueueEntity> MatchQueue => Set<MatchQueueEntity>();
        public DbSet<MatchQueueUser> MatchQueueUsers => Set<MatchQueueUser>();
        public DbSet<MatchQueueMessage> MatchQueueMessages => Set<MatchQueueMessage>();

        partial void OnModelCreatingMatchQueue(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MatchQueueEntity>(e =>
            {
                e.HasMany(x => x.Users)
                .WithOne();
                e.Navigation(x => x.Users).AutoInclude();

                e.HasOne(x => x.Match)
                .WithOne()
                .IsRequired(false)
                .HasForeignKey<MatchQueueEntity>("MatchId");
            });

            modelBuilder.Entity<MatchQueueUser>(e =>
            {
                e.Property(x => x.UserId).HasConversion(new ApplicationDiscordUserIdValueConvertor());
            });

            modelBuilder.Entity<MatchQueueMessage>(e =>
            {
                e.HasOne(p => p.MatchQueue)
                .WithOne()
                .HasForeignKey<MatchQueueMessage>("MatchQueueId");
            });
        }
    }
}
