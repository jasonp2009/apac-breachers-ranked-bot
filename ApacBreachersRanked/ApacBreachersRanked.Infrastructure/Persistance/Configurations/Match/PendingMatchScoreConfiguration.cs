using ApacBreachersRanked.Application.Match.Models;
using ApacBreachersRanked.Application.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApacBreachersRanked.Infrastructure.Persistance.Configurations.Match;

public class PendingMatchScoreConfiguration : IEntityTypeConfiguration<PendingMatchScore>
{
    public void Configure(EntityTypeBuilder<PendingMatchScore> builder)
    {
        builder.HasOne(p => p.Match)
            .WithMany()
            .HasForeignKey(p => p.MatchId);

        builder.OwnsOne(p => p.Score, score =>
        {
            score.OwnsMany(p => p.Maps, map =>
            {
                map.Ignore(p => p.Outcome);
            });
            score.Ignore(p => p.RoundScore);
            score.Ignore(p => p.MapScore);
            score.Ignore(p => p.Outcome);
        });

        builder.OwnsMany(p => p.Players, player =>
        {
            player.Property(p => p.UserId).HasConversion(new ApplicationDiscordUserIdValueConvertor());
        });
    }
}
