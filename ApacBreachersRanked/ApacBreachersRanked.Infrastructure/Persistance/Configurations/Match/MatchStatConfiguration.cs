using ApacBreachersRanked.Infrastructure.Breachers.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApacBreachersRanked.Infrastructure.Persistance.Configurations.Match;

public class MatchStatConfiguration : IEntityTypeConfiguration<MatchStatEntity>
{
    public void Configure(EntityTypeBuilder<MatchStatEntity> builder)
    {
        builder.OwnsMany(p => p.Games);
    }
}