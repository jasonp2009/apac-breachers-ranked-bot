namespace ApacBreachersRanked.Domain.Common
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public IList<IDomainEvent> DomainEvents { get; set; } = new List<IDomainEvent>();
        public void QueueDomainEvent(IDomainEvent domainEvent)
        {
            DomainEvents.Add(domainEvent);
        }
    }
}
