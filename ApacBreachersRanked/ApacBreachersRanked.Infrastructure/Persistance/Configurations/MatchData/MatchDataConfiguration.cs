using ApacBreachersRanked.Application.Users;
using ApacBreachersRanked.Domain.MatchData.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApacBreachersRanked.Infrastructure.Persistance.Configurations.MatchData;

public class MatchDataConfiguration : IEntityTypeConfiguration<GameDataEntity>
{
    public void Configure(EntityTypeBuilder<GameDataEntity> builder)
    {
        builder.HasOne(x => x.Match)
            .WithOne()
            .IsRequired()
            .HasForeignKey<GameDataEntity>(x => x.MatchId);
        builder.OwnsOne(x => x.Score);
        builder.OwnsMany(x => x.Players, player =>
        {
            player.Property(x => x.UserId).HasConversion(new ApplicationDiscordUserIdValueConvertor());
            player.OwnsMany(x => x.Rounds, round =>
            {
                round.Property(x => x.UserId).HasConversion(new ApplicationDiscordUserIdValueConvertor());
                round.OwnsMany(x => x.Weapons);
                round.OwnsMany(x => x.Gadgets);
            });
        });

        builder.Ignore(x => x.HomePlayers);
        builder.Ignore(x => x.AwayPlayers);
    }
}
