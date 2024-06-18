using ApacBreachersRanked.Application.MatchVote.Models;
using ApacBreachersRanked.Application.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApacBreachersRanked.Infrastructure.Persistance.Configurations.Match;

public class MatchVoteModelConfiguration : IEntityTypeConfiguration<MatchVoteModel>
{
    public void Configure(EntityTypeBuilder<MatchVoteModel> builder)
    {
        builder.HasOne(p => p.Match)
            .WithOne()
            .HasForeignKey<MatchVoteModel>(p => p.MatchId);

        builder.OwnsMany(p => p.HomeVotes, homeVote =>
        {
            homeVote.Property(x => x.UserId).HasConversion(new ApplicationDiscordUserIdValueConvertor());
        });
        builder.OwnsMany(p => p.AwayVotes, awayVote =>
        {
            awayVote.Property(x => x.UserId).HasConversion(new ApplicationDiscordUserIdValueConvertor());
        });
    }
}
