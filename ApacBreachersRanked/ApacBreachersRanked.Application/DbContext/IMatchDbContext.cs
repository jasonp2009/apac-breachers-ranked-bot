using ApacBreachersRanked.Application.Match.Models;
using ApacBreachersRanked.Domain.Match.Entities;
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
        public DbSet<MatchEntity> Matches { get; }
        public DbSet<MatchPlayer> MatchPlayers { get; }
        public DbSet<MatchThreads> MatchThreads { get; }
    }
}
