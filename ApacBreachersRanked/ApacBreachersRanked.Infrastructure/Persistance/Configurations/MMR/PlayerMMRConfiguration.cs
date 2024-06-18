using ApacBreachersRanked.Application.Users;
using ApacBreachersRanked.Domain.MMR.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApacBreachersRanked.Infrastructure.Persistance.Configurations.MMR;

public class PlayerMMRConfiguration : IEntityTypeConfiguration<PlayerMMR>
{
    public void Configure(EntityTypeBuilder<PlayerMMR> builder)
    {
        builder.Property(x => x.UserId).HasConversion(new ApplicationDiscordUserIdValueConvertor());

        builder.HasMany(x => x.Adjustments)
            .WithOne()
            .HasForeignKey("PlayerMMRId")
            .HasPrincipalKey(x => x.Id);
    }
}
