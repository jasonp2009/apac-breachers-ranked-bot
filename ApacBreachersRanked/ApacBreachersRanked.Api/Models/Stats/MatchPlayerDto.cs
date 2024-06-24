using ApacBreachersRanked.Domain.Match.Enums;
using ApacBreachersRanked.Domain.MMR.Enums;

namespace ApacBreachersRanked.Api.Models.Stats;

public class MatchPlayerDto
{
    public ulong UserId { get; private set; }
    public string Name { get; private set; }
    public decimal MMR { get; private set; }
    public Rank? Rank { get; private set; }
    public MatchSide Side { get; private set; }
    public bool Confirmed { get; private set; } = false;
    public bool IsHost { get; private set; } = false;
}
