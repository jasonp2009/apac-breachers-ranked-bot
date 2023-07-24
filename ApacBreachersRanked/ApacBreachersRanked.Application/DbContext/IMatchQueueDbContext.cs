using ApacBreachersRanked.Application.MatchQueue.Models;
using ApacBreachersRanked.Domain.MatchQueue.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApacBreachersRanked.Application.DbContext
{
    public partial interface IDbContext
    {
        DbSet<MatchQueueEntity> MatchQueue { get; }
        DbSet<MatchQueueUser> MatchQueueUsers { get; }
        DbSet<MatchQueueMessage> MatchQueueMessages { get; }
    }
}
