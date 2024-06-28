using ApacBreachersRanked.Domain.Match.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApacBreachersRanked.Infrastructure.Persistance.Configurations.Match;

public class MatchEntityConfiguration : IEntityTypeConfiguration<MatchEntity>
{
    public void Configure(EntityTypeBuilder<MatchEntity> builder)
    {
        builder.Property(p => p.MatchNumber).ValueGeneratedOnAdd();

        builder.HasMany(p => p.AllPlayers)
            .WithOne(p => p.Match);

        builder.Ignore(p => p.HomePlayers);
        builder.Ignore(p => p.HomeMMR);
        builder.Ignore(p => p.AwayPlayers);
        builder.Ignore(p => p.AwayMMR);
        builder.Ignore(p => p.HostPlayer);

        builder.OwnsOne(p => p.Score, score =>
        {
            score.OwnsMany(p => p.Maps, map =>
            {
                map.Ignore(p => p.Outcome);
                map.Ignore(p => p.MapName);
            });
            score.Ignore(p => p.RoundScore);
            score.Ignore(p => p.MapScore);
            score.Ignore(p => p.Outcome);
            score.ToTable("MatchScores");
        });

        builder.Navigation(p => p.Score);
    }
}
