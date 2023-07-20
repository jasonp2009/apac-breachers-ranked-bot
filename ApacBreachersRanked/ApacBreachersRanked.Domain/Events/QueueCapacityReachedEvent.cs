using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApacBreachersRanked.Domain.Events
{
    public class QueueCapacityReachedEvent : IDomainEvent
    {
        public Guid MatchQueueId { get; set; }
    }
}
