using ApacBreachersRanked.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApacBreachersRanked.Domain.MatchQueue.Events
{
    public class MatchQueueClosedEvent : IDomainEvent
    {
        public Guid MatchQueueId { get; set; }
    }
}
