using ApacBreachersRanked.Domain.Entities;
using ApacBreachersRanked.Domain.Events;
using ApacBreachersRanked.Domain.Repositories;
using MediatR;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace ApacBreachersRanked.Infrastructure.Repositories
{
    internal abstract class Repository<TEntity> : IRepository<TEntity>
        where TEntity : BaseEntity
    {
        protected readonly IMongoCollection<TEntity> _collection;
        protected readonly INotificationHandler<IDomainEvent> _notificationHandler;
        public Repository(
            IMongoDatabase mongoDatabase,
            INotificationHandler<IDomainEvent> notificationHandler)
        {
            _collection = mongoDatabase.GetCollection<TEntity>(CollectionName);
            RegisterClassMap();
            _notificationHandler = notificationHandler;
        }

        protected virtual string CollectionName => typeof(TEntity).Name.Replace("Entity", "");
        public IQueryable<TEntity> Query => _collection.AsQueryable();

        public virtual async Task SaveAsync(TEntity entity, CancellationToken cancellationToken)
        {
            await _collection.ReplaceOneAsync(x => x.Id == entity.Id, entity, new ReplaceOptions { IsUpsert = true }, cancellationToken);
            await HandleEvents(entity, cancellationToken);
        }


        public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken)
        {
            await HandleEvents(entity, cancellationToken);
            await _collection.DeleteOneAsync(x => x.Id == entity.Id);
        }

        protected virtual Task HandleEvents(TEntity entity, CancellationToken cancellationToken)
            => Task.WhenAll(entity.DomainEvents.Select(domainEvent => _notificationHandler.Handle(domainEvent, cancellationToken)));

        protected virtual void RegisterClassMap()
        {
            BsonClassMap.RegisterClassMap<TEntity>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                cm.UnmapMember(x => x.DomainEvents);
            });
        }
    }
}
