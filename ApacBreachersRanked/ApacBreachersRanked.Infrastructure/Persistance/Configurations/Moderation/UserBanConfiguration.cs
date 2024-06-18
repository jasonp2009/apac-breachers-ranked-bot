using ApacBreachersRanked.Application.Moderation.Models;
using ApacBreachersRanked.Application.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ApacBreachersRanked.Infrastructure.Persistance.Configurations.Moderation;

public class UserBanConfiguration : IEntityTypeConfiguration<UserBan>
{
    public void Configure(EntityTypeBuilder<UserBan> builder)
    {
        builder.Property(p => p.UserId).HasConversion(new ApplicationDiscordUserIdValueConvertor());

        builder.Property(p => p.Duration).HasConversion(new TimeSpanToTicksConverter());
    }
}
