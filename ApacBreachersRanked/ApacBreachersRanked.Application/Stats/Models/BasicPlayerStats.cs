using ApacBreachersRanked.Domain.User.Interfaces;

namespace ApacBreachersRanked.Application.Stats.Models
{
    public class BasicPlayerStats
    {
        public IUser User { get; internal set; }
        public decimal MMR { get; internal set; }
        public BasicMatchPlayerStats Match { get; internal set; }
    }
}
