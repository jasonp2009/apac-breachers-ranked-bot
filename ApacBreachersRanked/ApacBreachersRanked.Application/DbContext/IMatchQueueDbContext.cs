using ApacBreachersRanked.Application.MatchQueue;
using ApacBreachersRanked.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApacBreachersRanked.Application.DbContext
{
    public interface IMatchQueueDbContext : IDbContext
    {
        DbSet<MatchQueueEntity> MatchQueue { get; }
        DbSet<MatchQueueUser> MatchQueueUsers { get; }
        DbSet<MatchQueueMessage> MatchQueueMessages { get; }
    }
}
