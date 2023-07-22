using ApacBreachersRanked.Domain.Entities;
using ApacBreachersRanked.Infrastructure.Config;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Reflection;

namespace ApacBreachersRanked.Infrastructure.Persistance
{
    internal abstract class BaseDbContext : DbContext
    {
        protected readonly IPublisher _notificationHandler;
        private readonly RdsOptions _options;
        public BaseDbContext(
            IPublisher notificationHandler,
            IOptions<RdsOptions> options)
        {
            
            _notificationHandler = notificationHandler;
            _options = options.Value;
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(_options.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            IEnumerable<Type> entityTypes = Assembly.GetAssembly(typeof(BaseEntity)).GetTypes().Where(t => t.IsSubclassOf(typeof(BaseEntity)));
            foreach (Type entityType in entityTypes)
            {
                modelBuilder.Entity(entityType).Ignore(nameof(BaseEntity.DomainEvents));
            }
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            List<IDomainEvent> events = ChangeTracker.Entries<BaseEntity>()
                .Select(x => x.Entity.DomainEvents)
                .SelectMany(x => x)
                .ToList();

            int result = await base.SaveChangesAsync();

            await HandleEvents(events, cancellationToken);

            return result;
        }

        protected virtual Task HandleEvents(List<IDomainEvent> events, CancellationToken cancellationToken)
            => Task.WhenAll(events.Select(domainEvent => _notificationHandler.Publish(domainEvent, cancellationToken)));

    }
}
