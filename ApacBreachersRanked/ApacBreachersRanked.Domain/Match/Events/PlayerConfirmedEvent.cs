﻿using ApacBreachersRanked.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApacBreachersRanked.Domain.Match.Events
{
    public class PlayerConfirmedEvent : IDomainEvent
    {
        public Guid MatchId { get; set; }
        public Guid MatchPlayerId { get; set; }
    }
}
