using ApacBreachersRanked.Domain.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApacBreachersRanked.Infrastructure.ScheduledEventHandling
{
    internal class ScheduledEvent
    {
        public Guid Id { get; set; }
        public DateTime ScheduledForUtc { get; set; }
        public IScheduledEvent? Event { get; set; }
        public string SerialisedEvent
        {
            get
            {
                return JsonConvert.SerializeObject(Event, JsonSerializerSettings);
            }
            set
            {
                Event = JsonConvert.DeserializeObject<IScheduledEvent>(value, JsonSerializerSettings);
            }
        }

        private static JsonSerializerSettings JsonSerializerSettings => new()
        {
            TypeNameHandling = TypeNameHandling.All
        };

        private ScheduledEvent()
        {

        }

        public ScheduledEvent(IScheduledEvent scheduledEvent)
        {
            ScheduledForUtc = scheduledEvent.ScheduledForUtc;
            Event = scheduledEvent;
        }
    }
}
