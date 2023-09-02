using ApacBreachersRanked.Application.PingTimer.Models;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Application.DbContext
{
    public partial interface IDbContext
    {
        public DbSet<TimedPing> TimedPings { get; }
    }
}