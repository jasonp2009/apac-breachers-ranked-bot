using ApacBreachersRanked.Application.Users;
using ApacBreachersRanked.Domain.MatchData.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApacBreachersRanked.Infrastructure.Persistance.Configurations.MatchData;

public class MatchPlayerRoundDataConfiguration : IEntityTypeConfiguration<MatchPlayerRoundData>
{
    public void Configure(EntityTypeBuilder<MatchPlayerRoundData> builder)
    {
        builder.Property(x => x.UserId).HasConversion(new ApplicationDiscordUserIdValueConvertor());
        builder.OwnsMany(x => x.Weapons);
        builder.OwnsMany(x => x.Gadgets);
    }
}
