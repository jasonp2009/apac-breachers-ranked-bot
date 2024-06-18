using ApacBreachersRanked.Application.PingTimer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApacBreachersRanked.Infrastructure.Persistance.Configurations.Moderation;

public class TimedPingConfiguration : IEntityTypeConfiguration<TimedPing>
{
    public void Configure(EntityTypeBuilder<TimedPing> builder)
    {
        builder.HasKey(p => p.RoleId);
    }
}
