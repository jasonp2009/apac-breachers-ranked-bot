using ApacBreachersRanked.Domain.Match.Enums;

namespace ApacBreachersRanked.Domain.Match.Constants
{
    public static class MatchConstants
    {
        public static int MaxCapacity = 10;
        public static int MinCapacity = 6;
        public static int MaxTeamSize = 5;
        public static int MinTeamSize = 3;
        public static int AutoCancelMins = 3;

        public static List<Map> ValidMaps =
        [
            Map.Factory,
            Map.Skyscraper,
            Map.Hideout,
            Map.Ship,
            Map.Arctic,
            Map.Dam,
        ];
    }
}
