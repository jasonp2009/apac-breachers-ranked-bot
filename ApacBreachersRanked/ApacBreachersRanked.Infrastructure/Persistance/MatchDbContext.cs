using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.Match.Models;
using ApacBreachersRanked.Application.Users;
using ApacBreachersRanked.Domain.Entities;
using ApacBreachersRanked.Infrastructure.Config;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApacBreachersRanked.Infrastructure.Persistance
{
    internal partial class BreachersDbContext : IDbContext
    {

        public DbSet<MatchEntity> Matches => Set<MatchEntity>();
        public DbSet<MatchPlayer> MatchPlayers => Set<MatchPlayer>();
        public DbSet<MatchThreads> MatchThreads => Set<MatchThreads>();

        partial void OnModelCreatingMatch(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MatchEntity>(e =>
            {
                e.HasMany(p => p.HomePlayers)
                .WithOne();

                e.Navigation(p => p.HomePlayers).AutoInclude();

                e.HasMany(p => p.AwayPlayers)
                .WithOne();

                e.Navigation(p => p.AwayPlayers).AutoInclude();

                e.Ignore(p => p.AllPlayers);

                e.OwnsOne(p => p.Score);
            });

            modelBuilder.Entity<MatchPlayer>(e =>
            {
                e.Property(p => p.UserId).HasConversion(new ApplicationDiscordUserIdValueConvertor());
            });

            modelBuilder.Entity<MatchThreads>(e =>
            {
                e.HasOne(x => x.Match)
                .WithOne()
                .HasForeignKey<MatchThreads>("MatchId");
            });
        }
    }
}
