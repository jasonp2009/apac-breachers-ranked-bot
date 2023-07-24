﻿using ApacBreachersRanked.Domain.Common;
using ApacBreachersRanked.Infrastructure.Config;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Reflection;

namespace ApacBreachersRanked.Infrastructure.Persistance
{
    internal partial class BreachersDbContext : DbContext
    {
        protected readonly IPublisher _notificationHandler;
        private readonly RdsOptions _options;
        public BreachersDbContext(
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
            
            IEnumerable<Type> entityTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).Where(t => t.IsSubclassOf(typeof(BaseEntity)));
            foreach (Type entityType in entityTypes)
            {
                modelBuilder.Entity(entityType).Ignore(nameof(BaseEntity.DomainEvents));
            }
            OnModelCreatingMatchQueue(modelBuilder);
            OnModelCreatingMatch(modelBuilder);
        }

        partial void OnModelCreatingMatchQueue(ModelBuilder modelBuilder);
        partial void OnModelCreatingMatch(ModelBuilder modelBuilder);

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

        protected virtual Task HandleEvents(IEnumerable<IDomainEvent> events, CancellationToken cancellationToken)
            => Task.WhenAll(events.Select(domainEvent => _notificationHandler.Publish(domainEvent, cancellationToken)));

    }
}
