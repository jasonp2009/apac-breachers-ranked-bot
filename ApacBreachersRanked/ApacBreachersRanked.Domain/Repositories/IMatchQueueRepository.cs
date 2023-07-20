using ApacBreachersRanked.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApacBreachersRanked.Domain.Repositories
{
    public interface IMatchQueueRepository : IRepository<MatchQueueEntity>
    {
        public MatchQueueEntity CurrentQueue => Query.FirstOrDefault(x => x.IsOpen) ?? new();
    }
}
