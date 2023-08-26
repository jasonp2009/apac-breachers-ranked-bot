using ApacBreachersRanked.Domain.Helpers;
using ApacBreachersRanked.Domain.MMR.Enums;

namespace ApacBreachersRanked.Domain.MMR.Helpers
{
    public static class RankHelpers
    {
        public static bool IsInRange(this Rank rank, decimal MMR)
            => rank.GetAttributeOfType<RankRangeAttribute>().IsInRange(MMR);

        public static Rank GetRankForMMR(decimal MMR)
            => AttributeExtensions.GetEnumValuesWithAttribute<Rank, RankRangeAttribute>()
                    .First(x => x.Value.IsInRange(MMR))
                    .Key;
    }
}
