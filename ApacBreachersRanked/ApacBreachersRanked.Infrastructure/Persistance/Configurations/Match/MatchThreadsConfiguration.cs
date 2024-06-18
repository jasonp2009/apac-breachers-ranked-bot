using ApacBreachersRanked.Application.Match.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApacBreachersRanked.Infrastructure.Persistance.Configurations.Match;

public class MatchThreadsConfiguration : IEntityTypeConfiguration<MatchThreads>
{
    public void Configure(EntityTypeBuilder<MatchThreads> builder)
    {
        builder.HasOne(x => x.Match)
            .WithOne()
            .HasForeignKey<MatchThreads>("MatchId");
    }
}
