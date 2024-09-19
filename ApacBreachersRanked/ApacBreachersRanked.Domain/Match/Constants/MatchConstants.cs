using ApacBreachersRanked.Domain.Match.Enums;

namespace ApacBreachersRanked.Domain.Match.Constants
{
    public static class MatchConstants
    {
        public const MatchFormat DefaultMatchFormat = MatchFormat.Ranked2V2;

        public static readonly Dictionary<MatchFormat, MatchFormatConstants> MatchFormatConstants = new()
        {
            {
                MatchFormat.Ranked5V5,
                new()
                {
                    FriendlyName = "Ranked 5v5",
                    PingAtPlayers = 4,
                    MaxCapacity = 10,
                    MinCapacity = 6,
                    MaxTeamSize = 5,
                    MinTeamSize = 3,
                    AutoCancelMins = 3,
                    ValidMaps =
                    [
                        Map.Factory,
                        Map.Skyscraper,
                        Map.Hideout,
                        Map.Ship,
                        Map.Arctic,
                        Map.Dam,
                    ]
                }
            },
            {
                MatchFormat.Ranked2V2,
                new()
                {
                    FriendlyName = "Ranked 3v3",
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
                        Map.Dam,
                    ]
                }
            },
        };
    }
}
