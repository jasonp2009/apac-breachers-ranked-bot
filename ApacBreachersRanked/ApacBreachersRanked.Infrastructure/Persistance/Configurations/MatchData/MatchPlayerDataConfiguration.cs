using ApacBreachersRanked.Application.Users;
using ApacBreachersRanked.Domain.MatchData.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApacBreachersRanked.Infrastructure.Persistance.Configurations.MatchData;

public class MatchPlayerDataConfiguration : IEntityTypeConfiguration<MatchPlayerData>
{
    public void Configure(EntityTypeBuilder<MatchPlayerData> builder)
    {
        builder.Property(x => x.UserId).HasConversion(new ApplicationDiscordUserIdValueConvertor());
        builder.OwnsMany(x => x.Rounds);
    }
}
