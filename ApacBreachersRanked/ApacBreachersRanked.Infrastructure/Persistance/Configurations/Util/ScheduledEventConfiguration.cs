using ApacBreachersRanked.Infrastructure.ScheduledEventHandling;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApacBreachersRanked.Infrastructure.Persistance.Configurations.Util;

internal class ScheduledEventConfiguration : IEntityTypeConfiguration<ScheduledEvent>
{
    public void Configure(EntityTypeBuilder<ScheduledEvent> builder)
    {
        builder.Ignore(e => e.Event);
    }
}
