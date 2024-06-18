using ApacBreachersRanked.Application.Users;
using ApacBreachersRanked.Domain.MatchQueue.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApacBreachersRanked.Infrastructure.Persistance.Configurations.MatchQueue;

public class MatchQueueEntityConfiguration : IEntityTypeConfiguration<MatchQueueEntity>
{
    public void Configure(EntityTypeBuilder<MatchQueueEntity> builder)
    {
        builder.UseTpcMappingStrategy().ToTable("MatchQueue");
        builder.OwnsMany(x => x.Users, users =>
        {
            users.Property(x => x.UserId).HasConversion(new ApplicationDiscordUserIdValueConvertor());
        });

        builder.Navigation(x => x.Users);

        builder.HasOne(x => x.Match)
            .WithOne()
            .IsRequired(false)
            .HasForeignKey<MatchQueueEntity>("MatchId");
    }
}
