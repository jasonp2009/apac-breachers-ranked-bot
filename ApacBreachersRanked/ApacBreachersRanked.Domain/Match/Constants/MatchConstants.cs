using ApacBreachersRanked.Domain.Match.Enums;

namespace ApacBreachersRanked.Domain.Match.Constants;

public static class MatchConstants
{
    public const MatchFormat DefaultMatchFormat = MatchFormat.RankedShortStack;

    public static readonly Dictionary<MatchFormat, MatchFormatConstants> MatchFormatConstants = new()
    {
        {
            MatchFormat.Ranked,
            new MatchFormatConstants
            {
                FriendlyName = "Ranked",
                PingAtPlayers = 4,
                MaxCapacity = 10,
                MinCapacity = 8,
                MaxTeamSize = 5,
                MinTeamSize = 4,
                AutoCancelMins = 3,
                ValidMaps =
                [
                    Map.Factory,
                    Map.Skyscraper,
                    Map.Hideout,
                    Map.Ship,
                    Map.Arctic,
                    Map.Dam
                ]
            }
        },
        {
            MatchFormat.RankedShortStack,
            new MatchFormatConstants
            {
                FriendlyName = "Ranked Short Stack",
                PingAtPlayers = 3,
                MaxCapacity = 6,
                MinCapacity = 4,
                MaxTeamSize = 3,
                MinTeamSize = 2,
                AutoCancelMins = 3,
                ValidMaps =
                [
                    Map.Factory,
                    Map.Skyscraper,
                    Map.Hideout,
                    Map.Ship,
                    Map.Arctic,
                    Map.Dam
                ]
            }
        }
    };
}