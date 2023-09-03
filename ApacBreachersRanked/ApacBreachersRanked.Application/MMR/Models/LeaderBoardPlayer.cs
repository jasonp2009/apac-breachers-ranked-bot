using ApacBreachersRanked.Application.Stats.Models;
using ApacBreachersRanked.Domain.MMR.Entities;
using ApacBreachersRanked.Domain.MMR.Enums;
using ApacBreachersRanked.Domain.User.Interfaces;

namespace ApacBreachersRanked.Application.MMR.Models
{
    public class LeaderBoardPlayer : IUser
    {
        public IUserId UserId { get; private set; } = null!;
        public string? Name { get; set; }
        public decimal MMR { get; private set; } = 1000;
        public Rank? Rank { get; private set; }
        public BasicMatchPlayerStats Matches { get; set; }
        public LeaderBoardPlayer(PlayerMMR playerMMR, BasicMatchPlayerStats basicStats)
        {
            UserId = playerMMR.UserId;
            Name = playerMMR.Name;
            MMR = playerMMR.MMR;
            Rank = playerMMR.Rank;
            Matches = basicStats;
        }
    }
}
