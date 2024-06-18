using ApacBreachersRanked.Infrastructure.Breachers.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApacBreachersRanked.Infrastructure.Persistance.Configurations.BreachersApi;

public class BreachersDiscordUserLinkConfiguration : IEntityTypeConfiguration<BreachersDiscordUserLink>
{
    public void Configure(EntityTypeBuilder<BreachersDiscordUserLink> builder)
    {
        builder.HasKey(p => p.DiscordUserId);
    }
}
