using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.Match.Models;
using ApacBreachersRanked.Application.Users;
using ApacBreachersRanked.Domain.Match.Entities;
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
        public DbSet<MapScore> MatchMaps => Set<MapScore>();
        public DbSet<MatchThreads> MatchThreads => Set<MatchThreads>();
        public DbSet<PendingMatchScore> PendingMatchScores => Set<PendingMatchScore>();

        partial void OnModelCreatingMatch(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MatchEntity>(e =>
            {
                e.Property(p => p.MatchNumber).ValueGeneratedOnAdd();

                e.HasMany(p => p.AllPlayers)
                .WithOne(p => p.Match);

                e.Navigation(p => p.AllPlayers).AutoInclude();

                e.Ignore(p => p.HomePlayers);
                e.Ignore(p => p.AwayPlayers);
                e.Ignore(p => p.HostPlayer);

                e.OwnsOne(p => p.Score, score =>
                {
                    score.OwnsMany(p => p.Maps, map =>
                    {
                        map.Ignore(p => p.Outcome);
                    });
                    score.Ignore(p => p.RoundScore);
                    score.Ignore(p => p.MapScore);
                    score.Ignore(p => p.Outcome);
                    score.ToTable("MatchScores");
                });

                e.Navigation(p => p.Score).AutoInclude();
            });

            modelBuilder.Entity<MatchPlayer>(e =>
            {
                e.Navigation(p => p.Match).AutoInclude();
                e.Property(p => p.UserId).HasConversion(new ApplicationDiscordUserIdValueConvertor());
            });

            modelBuilder.Entity<MatchThreads>(e =>
            {
                e.HasOne(x => x.Match)
                .WithOne()
                .HasForeignKey<MatchThreads>("MatchId");
            });

            modelBuilder.Entity<PendingMatchScore>(e =>
            {
                e.HasOne(p => p.Match)
                .WithMany()
                .HasForeignKey(p => p.MatchId);

                e.OwnsOne(p => p.Score, score =>
                {
                    score.OwnsMany(p => p.Maps, map =>
                    {
                        map.Ignore(p => p.Outcome);
                    });
                    score.Ignore(p => p.RoundScore);
                    score.Ignore(p => p.MapScore);
                    score.Ignore(p => p.Outcome);
                });

                e.OwnsMany(p => p.Players, player =>
                {
                    player.Property(p => p.UserId).HasConversion(new ApplicationDiscordUserIdValueConvertor());
                });
            });
        }
    }
}
