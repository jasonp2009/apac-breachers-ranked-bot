using ApacBreachersRanked.Application.Users;
using ApacBreachersRanked.Domain.MMR.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApacBreachersRanked.Infrastructure.Persistance.Configurations.MMR;

public class MMRAdjustmentConfiguration : IEntityTypeConfiguration<MMRAdjustment>
{
    public void Configure(EntityTypeBuilder<MMRAdjustment> builder)
    {
        builder.Property<int>("Id")
            .HasColumnType("int")
            .ValueGeneratedOnAdd();
        builder.HasKey("Id");

        builder.Property(x => x.UserId).HasConversion(new ApplicationDiscordUserIdValueConvertor());
    }
}
