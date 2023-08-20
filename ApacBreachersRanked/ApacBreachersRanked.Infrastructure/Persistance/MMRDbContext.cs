using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.MMR.Models;
using ApacBreachersRanked.Application.Users;
using ApacBreachersRanked.Domain.MMR.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Infrastructure.Persistance
{
    internal partial class BreachersDbContext : IDbContext
    {
        public DbSet<PlayerMMR> PlayerMMRs => Set<PlayerMMR>();
        public DbSet<MMRAdjustment> MMRAdjustments => Set<MMRAdjustment>();
        public DbSet<LeaderBoardMessage> LeaderBoardMessages => Set<LeaderBoardMessage>();

        public async Task ResetMMRAsync()
        {
            await Database.ExecuteSqlRawAsync($"DELETE [{Model.FindEntityType(typeof(MMRAdjustment))?.GetTableName()}]");
            await Database.ExecuteSqlRawAsync($"DELETE [{Model.FindEntityType(typeof(PlayerMMR))?.GetTableName()}]");
        }

        partial void OnModelCreatingMMR(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlayerMMR>(e =>
            {
                e.Property(x => x.UserId).HasConversion(new ApplicationDiscordUserIdValueConvertor());

                e.HasMany(x => x.Adjustments)
                .WithOne()
                .HasForeignKey("PlayerMMRId")
                .HasPrincipalKey(x => x.Id);
            });

            modelBuilder.Entity<MMRAdjustment>(e =>
            {
                e.Property<int>("Id")
                .HasColumnType("int")
                .ValueGeneratedOnAdd();
                e.HasKey("Id");

                e.Property(x => x.UserId).HasConversion(new ApplicationDiscordUserIdValueConvertor());
            });
        }
    }
}
