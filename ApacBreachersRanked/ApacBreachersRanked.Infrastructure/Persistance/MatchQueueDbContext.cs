using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.MatchQueue;
using ApacBreachersRanked.Application.Users;
using ApacBreachersRanked.Domain.Entities;
using ApacBreachersRanked.Infrastructure.Config;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace ApacBreachersRanked.Infrastructure.Persistance
{
    internal class MatchQueueDbContext : BaseDbContext, IMatchQueueDbContext
    {
        public MatchQueueDbContext(
            IPublisher notificationHandler,
            IOptions<RdsOptions> options)
        : base(notificationHandler, options)
        {
        }

        public DbSet<MatchQueueEntity> MatchQueue => Set<MatchQueueEntity>();
        public DbSet<MatchQueueUser> MatchQueueUsers => Set<MatchQueueUser>();
        public DbSet<MatchQueueMessage> MatchQueueMessages => Set<MatchQueueMessage>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MatchQueueEntity>(e =>
            {
                e.HasMany(x => x.Users)
                .WithOne();
                e.Navigation(x => x.Users).AutoInclude();
            });

            modelBuilder.Entity<MatchQueueUser>(e =>
            {
                e.Property(x => x.UserId).HasConversion(new ApplicationDiscordUserIdValueConvertor());
            });

            modelBuilder.Entity<MatchQueueMessage>(e =>
            {
                e.HasKey(p => p.MatchQueueId);
                e.HasOne(p => p.MatchQueue)
                .WithOne()
                .HasForeignKey<MatchQueueMessage>(x => x.MatchQueueId);
            });
        }
    }
}
