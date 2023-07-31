using ApacBreachersRanked.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApacBreachersRanked.Domain.Match.Events
{
    public class AutoCancelMatchEvent : IScheduledEvent
    {
        public DateTime ScheduledForUtc { get; set; }
        public Guid MatchId { get; set; }
    }
}
