using ApacBreachersRanked.Application.MatchQueue.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApacBreachersRanked.Infrastructure.Persistance.Configurations.MatchQueue;

public class MatchQueueMessageConfiguration : IEntityTypeConfiguration<MatchQueueMessage>
{
    public void Configure(EntityTypeBuilder<MatchQueueMessage> builder)
    {
        builder.HasOne(p => p.MatchQueue)
            .WithOne()
            .HasForeignKey<MatchQueueMessage>("MatchQueueId");
    }
}
