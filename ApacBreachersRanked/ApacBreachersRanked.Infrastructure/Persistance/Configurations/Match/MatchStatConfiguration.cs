using ApacBreachersRanked.Infrastructure.Breachers.Entities;
using ApacBreachersRanked.Infrastructure.Breachers.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;

namespace ApacBreachersRanked.Infrastructure.Persistance.Configurations.Match;

public class MatchStatConfiguration : IEntityTypeConfiguration<MatchDataEntity>
{
    public void Configure(EntityTypeBuilder<MatchDataEntity> builder)
    {
        builder.Property(p => p.Games)
            .HasConversion(new BreachersGamesValueConvertor());
    }
}

public class BreachersGamesValueConvertor : ValueConverter<IEnumerable<GetMatchResponse>, string>
{
    public BreachersGamesValueConvertor() : base(
        games => JsonConvert.SerializeObject(games),
        v => JsonConvert.DeserializeObject<IEnumerable<GetMatchResponse>>(v)
    )
    { }
}