using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApacBreachersRanked.Domain.Common
{
    public interface IScheduledEvent : IDomainEvent
    {
        public DateTime ScheduledForUtc { get; }
    }
}
