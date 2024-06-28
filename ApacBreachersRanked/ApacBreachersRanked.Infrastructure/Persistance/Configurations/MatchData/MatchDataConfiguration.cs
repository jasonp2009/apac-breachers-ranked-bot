using ApacBreachersRanked.Application.Users;
using ApacBreachersRanked.Domain.MatchData.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApacBreachersRanked.Infrastructure.Persistance.Configurations.MatchData;

public class MatchDataConfiguration : IEntityTypeConfiguration<GameDataEntity>
{
    public void Configure(EntityTypeBuilder<GameDataEntity> builder)
    {
        builder.OwnsOne(x => x.Score);
        builder.OwnsMany(x => x.Players, player =>
        {
            player.Property(x => x.UserId).HasConversion(new ApplicationDiscordUserIdValueConvertor());
            player.OwnsMany(x => x.Rounds, round =>
            {
                round.Property(x => x.UserId).HasConversion(new ApplicationDiscordUserIdValueConvertor());
                round.OwnsMany(x => x.Weapons, weapon =>
                {
                    weapon.Ignore(x => x.Name);
                });
                round.OwnsMany(x => x.Gadgets, gadget =>
                {
                    gadget.Ignore(x => x.Name);
                });
            });
            player.Ignore(x => x.Weapons);
            player.Ignore(x => x.Gadgets);
        });

        builder.Ignore(x => x.HomePlayers);
        builder.Ignore(x => x.AwayPlayers);
        builder.Ignore(x => x.Rounds);
    }
}
