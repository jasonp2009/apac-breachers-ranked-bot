using ApacBreachersRanked.Domain.MMR.Entities;

namespace ApacBreachersRanked.Application.MMR.Extensions;

public static class MmrExtensions
{
    public static void ApplyAdjustmentsToPlayerMmrs(this List<PlayerMMR> playerMmRs, List<MMRAdjustment> adjustments)
    {
        foreach (var adjustment in adjustments)
        {
            var playerMmr = playerMmRs.FirstOrDefault(x => x.UserId.Equals(adjustment.UserId));
            if (playerMmr != null) playerMmr.ApplyAdjustment(adjustment);
        }
    }
    
    public static decimal CalculateExpected(decimal forMmr, decimal againstMmr)
    {
        return 1 / (1 + Convert.ToDecimal(Math.Pow(10, (double)(againstMmr - forMmr) / 400)));
    }
    
    public static decimal CalculateAdjustment(decimal kFactor, decimal expected, decimal actual)
    {
        return kFactor * (actual - expected);
    }
}