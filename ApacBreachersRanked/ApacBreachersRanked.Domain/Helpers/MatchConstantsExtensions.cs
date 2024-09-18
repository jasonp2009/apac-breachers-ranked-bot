using ApacBreachersRanked.Domain.Match.Constants;
using ApacBreachersRanked.Domain.Match.Enums;

namespace ApacBreachersRanked.Domain.Helpers;

public static class MatchConstantsExtensions
{
    public static TValue GetMatchConstant<TValue>(this MatchFormat matchFormat, Func<MatchFormatConstants, TValue> selector)
    {
        var matchFormatConstants = matchFormat.GetMatchFormatConstants();
        if (matchFormatConstants == default)
        {
            return default;
        }
        return selector(matchFormatConstants);
    }

    public static MatchFormatConstants GetMatchFormatConstants(this MatchFormat matchFormat)
    {
        if (!MatchConstants.MatchFormatConstants.TryGetValue(matchFormat,
                out MatchFormatConstants matchFormatConstants))
        {
            if (!MatchConstants.MatchFormatConstants.TryGetValue(matchFormat,
                    out matchFormatConstants))
            {
                return default;
            }
        }

        return matchFormatConstants;
    }

    public static IEnumerable<MatchFormat> GetEnabledMatchFormats()
    {
        return MatchConstants.MatchFormatConstants.Where(pairs => pairs.Value.IsEnabled)
            .Select(pairs => pairs.Key);
    }

    public static string GetFriendlyName(this MatchFormat matchFormat)
        => matchFormat.GetMatchFormatConstants().FriendlyName;
}