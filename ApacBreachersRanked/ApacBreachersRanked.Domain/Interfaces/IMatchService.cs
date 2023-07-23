using ApacBreachersRanked.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApacBreachersRanked.Domain.Interfaces
{
    public interface IMatchService
    {
        public Task<MatchEntity> CreateMatchFromQueue(MatchQueueEntity matchQueue);
    }
}
