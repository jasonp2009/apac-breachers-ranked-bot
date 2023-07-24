using ApacBreachersRanked.Domain.Common;
using ApacBreachersRanked.Domain.MatchQueue.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApacBreachersRanked.Application.MatchQueue.Models
{
    public class MatchQueueMessage : BaseEntity
    {
        public MatchQueueEntity MatchQueue { get; set; } = null!;
        public ulong DiscordMessageId { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
