using ApacBreachersRanked.Domain.Common;
using ApacBreachersRanked.Infrastructure.Config;
using ApacBreachersRanked.Infrastructure.ScheduledEventHandling;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Text.Json;

namespace ApacBreachersRanked.Infrastructure.Persistance
{
    internal partial class BreachersDbContext : DbContext
    {
        private readonly IServiceProvider _services;
        private readonly ScheduledEventHandlingService _scheduledEventHandlerService;
        private readonly RdsOptions _options;
        public BreachersDbContext(
            IServiceProvider services,
            ScheduledEventHandlingService scheduledEventHandlingService,
            IOptions<RdsOptions> options)
        {
            _services = services;
            _scheduledEventHandlerService = scheduledEventHandlingService;
            _options = options.Value;
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(_options.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            IEnumerable<Type> entityTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).Where(t => t.IsSubclassOf(typeof(BaseEntity)));
            foreach (Type entityType in entityTypes)
            {
                modelBuilder.Entity(entityType).Ignore(nameof(BaseEntity.DomainEvents));
            }
            OnModelCreatingMatchQueue(modelBuilder);
            OnModelCreatingMatch(modelBuilder);
            OnModelCreatingMatchVote(modelBuilder);
            OnModelCreatingScheduledEvent(modelBuilder);
            OnModelCreatingMMR(modelBuilder);
            OnModelCreatingModeration(modelBuilder);
        }

        partial void OnModelCreatingMatchQueue(ModelBuilder modelBuilder);
        partial void OnModelCreatingMatch(ModelBuilder modelBuilder);
        partial void OnModelCreatingMatchVote(ModelBuilder modelBuilder);
        partial void OnModelCreatingScheduledEvent(ModelBuilder modelBuilder);
        partial void OnModelCreatingMMR(ModelBuilder modelBuilder);
        partial void OnModelCreatingModeration(ModelBuilder modelBuilder);

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            
            List<BaseEntity> entities = ChangeTracker.Entries<BaseEntity>().Select(x => x.Entity).ToList();

            List<IDomainEvent> events = entities.SelectMany(x => x.DomainEvents).ToList();

            int result = await base.SaveChangesAsync();

            foreach (var entity in entities)
            {
                entity.DomainEvents.Clear();
            }

            await HandleEvents(events, cancellationToken);

            return result;
        }

        protected virtual async Task HandleEvents(IEnumerable<IDomainEvent> events, CancellationToken cancellationToken)
        {
            List<int> scheduledHashes = new();
            foreach (IDomainEvent domainEvent in events)
            {
                int eventHash = GetDomainEventHash(domainEvent);
                if (scheduledHashes.Contains(eventHash)) continue;

                scheduledHashes.Add(eventHash);
                _scheduledEventHandlerService.ScheduleEvent(domainEvent);
            }
        }

        private int GetDomainEventHash(IDomainEvent domainEvent)
        {
            return HashCode.Combine(domainEvent.GetType().FullName, JsonSerializer.Serialize(domainEvent));
        }
    }
}
