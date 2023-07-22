using ApacBreachersRanked.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApacBreachersRanked.Application.MatchQueue
{
    public class MatchQueueMessage
    {
        public Guid MatchQueueId { get; set; }
        public MatchQueueEntity MatchQueue { get; set; } = null!;
        public ulong DiscordMessageId { get; set; }
    }
}
