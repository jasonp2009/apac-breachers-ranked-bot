using ApacBreachersRanked.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApacBreachersRanked.Application.Match.Models
{
    public class MatchThreads : BaseEntity
    {
        public MatchEntity Match { get; set; }
        public ulong MatchThreadId { get; set; }
        public ulong HomeThreadId { get; set; }
        public ulong AwayThreadId { get; set; }

        private MatchThreads() { }
        public MatchThreads(MatchEntity match)
        {
            Match = match;
        }
    }
}
