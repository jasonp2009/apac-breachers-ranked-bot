using ApacBreachersRanked.Domain.Match.Enums;

namespace ApacBreachersRanked.Domain.Match.Constants;

public class MatchFormatConstants
{
    public bool IsEnabled { get; init; } = true;
    public string FriendlyName { get; init; }
    public int PingAtPlayers { get; init; } = 4;
    public int MaxCapacity { get; init; }
    public int MinCapacity { get; init; }
    public int MaxTeamSize { get; init; }
    public int MinTeamSize { get; init; }
    public int AutoCancelMins { get; init; }
    public List<Map> ValidMaps { get; init; }
}