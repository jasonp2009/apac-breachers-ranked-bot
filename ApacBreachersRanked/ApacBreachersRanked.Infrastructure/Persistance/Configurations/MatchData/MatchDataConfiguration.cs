using ApacBreachersRanked.Domain.MatchData.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApacBreachersRanked.Infrastructure.Persistance.Configurations.MatchData;

public class MatchDataConfiguration : IEntityTypeConfiguration<MatchDataEntity>
{
    public void Configure(EntityTypeBuilder<MatchDataEntity> builder)
    {
        builder.HasOne(x => x.Match)
            .WithOne()
            .IsRequired()
            .HasForeignKey<MatchDataEntity>(x => x.MatchId);
        builder.OwnsOne(x => x.Score);
        builder.OwnsMany(x => x.Players);
    }
}
