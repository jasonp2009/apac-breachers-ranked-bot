using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.MatchVote.Models;
using ApacBreachersRanked.Application.Users;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Infrastructure.Persistance
{
    internal partial class BreachersDbContext : IDbContext
    {
        public DbSet<MatchVoteModel> MatchVotes => Set<MatchVoteModel>();

        partial void OnModelCreatingMatchVote(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MatchVoteModel>(e =>
            {
                e.HasOne(p => p.Match)
                .WithOne()
                .HasForeignKey<MatchVoteModel>(p => p.MatchId);

                e.OwnsMany(p => p.HomeVotes, homeVote =>
                {
                    homeVote.Property(x => x.UserId).HasConversion(new ApplicationDiscordUserIdValueConvertor());
                });
                e.OwnsMany(p => p.AwayVotes, awayVote =>
                {
                    awayVote.Property(x => x.UserId).HasConversion(new ApplicationDiscordUserIdValueConvertor());
                });
            });
        }
    }
}
