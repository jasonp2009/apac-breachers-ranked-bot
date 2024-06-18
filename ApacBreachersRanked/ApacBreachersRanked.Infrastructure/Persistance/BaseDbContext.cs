using System.Reflection;
using ApacBreachersRanked.Domain.Common;
using ApacBreachersRanked.Infrastructure.Config;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text.Json;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.Match.Models;
using ApacBreachersRanked.Application.MatchQueue.Models;
using ApacBreachersRanked.Application.MatchVote.Models;
using ApacBreachersRanked.Application.MMR.Models;
using ApacBreachersRanked.Application.Moderation.Models;
using ApacBreachersRanked.Application.PingTimer.Models;
using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.MatchQueue.Entities;
using ApacBreachersRanked.Domain.MMR.Entities;
using ApacBreachersRanked.Infrastructure.Breachers.Entities;
using ApacBreachersRanked.Infrastructure.ScheduledEventHandling;

namespace ApacBreachersRanked.Infrastructure.Persistance
{
    internal class BreachersDbContext : DbContext, IDbContext
    {
        private readonly IMediator _mediator;
        private readonly RdsOptions _options;
        public BreachersDbContext(
            IMediator mediator,
            IOptions<RdsOptions> options)
        {
            _mediator = mediator;
            _options = options.Value;
            Database.EnsureCreated();
        }
        
        internal DbSet<BreachersDiscordUserLink> BreachersDiscordUserLinks => Set<BreachersDiscordUserLink>();
        public DbSet<MatchEntity> Matches => Set<MatchEntity>();
        public DbSet<MatchPlayer> MatchPlayers => Set<MatchPlayer>();
        public DbSet<MapScore> MatchMaps => Set<MapScore>();
        public DbSet<MatchThreads> MatchThreads => Set<MatchThreads>();
        public DbSet<PendingMatchScore> PendingMatchScores => Set<PendingMatchScore>();
        public DbSet<MatchQueueEntity> MatchQueue => Set<MatchQueueEntity>();
        public DbSet<MatchQueueMessage> MatchQueueMessages => Set<MatchQueueMessage>();
        public DbSet<ScheduledMatchQueueEntity> ScheduleMatchQueues => Set<ScheduledMatchQueueEntity>();
        public DbSet<ScheduledMatchQueueMessage> ScheduledMatchQueueMessages => Set<ScheduledMatchQueueMessage>();
        public DbSet<MatchVoteModel> MatchVotes => Set<MatchVoteModel>();
        public DbSet<PlayerMMR> PlayerMMRs => Set<PlayerMMR>();
        public DbSet<MMRAdjustment> MMRAdjustments => Set<MMRAdjustment>();
        public DbSet<LeaderBoardMessage> LeaderBoardMessages => Set<LeaderBoardMessage>();
        public DbSet<UserBan> UserBans => Set<UserBan>();
        public DbSet<ActiveBansMessage> ActiveBansMessages => Set<ActiveBansMessage>();
        public DbSet<TimedPing> TimedPings => Set<TimedPing>();
        internal DbSet<ScheduledEvent> ScheduledEvents => Set<ScheduledEvent>();

        public async Task ResetMMRAsync()
        {
            if (_options.DatabaseEngine == DatabaseEngine.SqlServer)
            {
                await Database.ExecuteSqlRawAsync($"DELETE [{Model.FindEntityType(typeof(MMRAdjustment))?.GetTableName()}]");
                await Database.ExecuteSqlRawAsync($"DELETE [{Model.FindEntityType(typeof(PlayerMMR))?.GetTableName()}]");
            } else if (_options.DatabaseEngine == DatabaseEngine.Postgress)
            {
                await MMRAdjustments.ExecuteDeleteAsync();
                await PlayerMMRs.ExecuteDeleteAsync();
            }
        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            switch(_options.DatabaseEngine)
            {
                case DatabaseEngine.SqlServer:
                    options.UseSqlServer(_options.ConnectionString);
                    break;
                case DatabaseEngine.Postgress:
                    options.UseNpgsql(_options.ConnectionString);
                    break;
                default:
                    throw new NotImplementedException($"Database engine {_options.DatabaseEngine} is not supported");
            };
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            IEnumerable<Type> entityTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).Where(t => t.IsSubclassOf(typeof(BaseEntity)));
            foreach (Type entityType in entityTypes)
            {
                modelBuilder.Entity(entityType).Ignore(nameof(BaseEntity.DomainEvents));
            }
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

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
                await _mediator.Publish(domainEvent, cancellationToken);
            }
        }

        private int GetDomainEventHash(IDomainEvent domainEvent)
        {
            return HashCode.Combine(domainEvent.GetType().FullName, JsonSerializer.Serialize(domainEvent));
        }
    }
}
