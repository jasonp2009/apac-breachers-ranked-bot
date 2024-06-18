using ApacBreachersRanked.Application.MatchQueue.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApacBreachersRanked.Infrastructure.Persistance.Configurations.MatchQueue;

public class ScheduledMatchQueueMessageConfiguration : IEntityTypeConfiguration<ScheduledMatchQueueMessage>
{
    public void Configure(EntityTypeBuilder<ScheduledMatchQueueMessage> builder)
    {
        builder.ToTable("ScheduledMatchQueueMessages");
        builder.HasOne(p => p.MatchQueue)
            .WithOne()
            .HasForeignKey<ScheduledMatchQueueMessage>("MatchQueueId");
    }
}
