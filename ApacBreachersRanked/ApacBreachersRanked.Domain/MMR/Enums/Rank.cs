namespace ApacBreachersRanked.Domain.MMR.Enums
{
    public enum Rank
    {
        [RankRange(1090, 1150)]
        Diamond = 1120,

        [RankRange(1030, 1090)]
        Gold = 1060,

        [RankRange(970, 1030)]
        Silver = 1000,

        [RankRange(910, 970)]
        Bronze = 940,

        [RankRange(850, 910)]
        Copper = 880
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class RankRangeAttribute : Attribute
    {
        public decimal MinMMR { get; set; }
        public decimal MaxMMR { get; set; }

        public RankRangeAttribute(int minMMR, int maxMMR)
        {
            MinMMR = minMMR;
            MaxMMR = maxMMR;
        }

        public bool IsInRange(decimal mmr)
            => (MinMMR <= mmr && mmr < MaxMMR);
    }
}
