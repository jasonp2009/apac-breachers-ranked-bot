using ApacBreachersRanked.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApacBreachersRanked.Domain.Entities
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }
        public IList<IDomainEvent> DomainEvents { get; set; } = new List<IDomainEvent>();
        public void QueueDomainEvent(IDomainEvent domainEvent)
        {
            DomainEvents.Add(domainEvent);
        }
    }
}
