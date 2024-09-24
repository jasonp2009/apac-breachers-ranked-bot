using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.Match.Enums;
using ApacBreachersRanked.Domain.User.Interfaces;

namespace ApacBreachersRanked.Domain.MMR.Entities;

public class MMRAdjustment
{
    private MMRAdjustment()
    {
    }

    public MMRAdjustment(IUserId userId, MatchFormat matchFormat, decimal adjustment, MatchEntity match)
    {
        UserId = userId;
        MatchFormat = matchFormat;
        Adjustment = adjustment;
        Match = match;
    }

    public IUserId UserId { get; private set; }
    public MatchFormat MatchFormat { get; private set; }
    public decimal Adjustment { get; private set; }
    public MatchEntity Match { get; private set; }
}