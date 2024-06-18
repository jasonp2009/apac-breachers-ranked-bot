using ApacBreachersRanked.Application.Users;
using ApacBreachersRanked.Domain.Match.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApacBreachersRanked.Infrastructure.Persistance.Configurations.Match;

public class MatchPlayerConfiguration : IEntityTypeConfiguration<MatchPlayer>
{
    public void Configure(EntityTypeBuilder<MatchPlayer> builder)
    {
        builder.Navigation(p => p.Match);
        builder.Property(p => p.UserId).HasConversion(new ApplicationDiscordUserIdValueConvertor());
    }
}
