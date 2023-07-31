using ApacBreachersRanked.Domain.Common;
using ApacBreachersRanked.Domain.User.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApacBreachersRanked.Domain.MatchQueue.Events
{
    public class MatchQueueUserExpiredEvent : IScheduledEvent
    {
        public DateTime ScheduledForUtc { get; set; }
        public IUserId MatchQueueUserId { get; set; }
    }
}
