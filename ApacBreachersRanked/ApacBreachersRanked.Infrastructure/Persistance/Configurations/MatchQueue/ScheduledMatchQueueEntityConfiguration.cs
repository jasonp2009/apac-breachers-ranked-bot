using ApacBreachersRanked.Application.Users;
using ApacBreachersRanked.Domain.MatchQueue.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApacBreachersRanked.Infrastructure.Persistance.Configurations.MatchQueue;

public class ScheduledMatchQueueEntityConfiguration : IEntityTypeConfiguration<ScheduledMatchQueueEntity>
{
    public void Configure(EntityTypeBuilder<ScheduledMatchQueueEntity> builder)
    {
        builder.UseTpcMappingStrategy().ToTable("ScheduledMatchQueues");
        builder.OwnsMany(x => x.Users, users =>
        {
            users.Property(x => x.UserId).HasConversion(new ApplicationDiscordUserIdValueConvertor());
        });

        builder.Navigation(x => x.Users);
    }
}
