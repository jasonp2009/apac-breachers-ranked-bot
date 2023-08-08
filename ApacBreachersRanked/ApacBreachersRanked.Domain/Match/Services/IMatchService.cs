using ApacBreachersRanked.Domain.Helpers;
using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.MatchQueue.Entities;
using ApacBreachersRanked.Domain.MMR.Entities;
using ApacBreachersRanked.Domain.User.Interfaces;

namespace ApacBreachersRanked.Domain.Match.Services
{
    public interface IMatchService
    {
        protected Task<List<PlayerMMR>> GetPlayerMMRsAsync(IEnumerable<IUser> users, CancellationToken cancellationToken = default);
        public async Task<MatchEntity> CreateMatchFromQueue(MatchQueueEntity matchQueue, CancellationToken cancellationToken)
        {
            List<IUser> users = matchQueue.Users.Select(user => user as IUser).ToList();

            List<PlayerMMR> playerMMRs = await GetPlayerMMRsAsync(users, cancellationToken);

            (List<IUser> home, List<IUser> away) = AllocateTeams(playerMMRs);

            return new MatchEntity(matchQueue, home, away);
        }

        public (List<IUser> Home, List<IUser> Away) AllocateTeams(List<PlayerMMR> playerMMRs)
        {
            playerMMRs = playerMMRs.OrderByDescending(x => x.MMR).ToList();

            (List<PlayerMMR> firstList, List<PlayerMMR> secondList) = DividePlayerMMRList(playerMMRs);

            List<IUser> Home, Away;

            if (RandomExtensions.RandomNumber(0,1) == 1)
            {
                Home = firstList.Select(player => player as IUser).ToList();
                Away = secondList.Select(player => player as IUser).ToList();
            }
            else
            {
                Away = firstList.Select(player => player as IUser).ToList();
                Home = secondList.Select(player => player as IUser).ToList();
            }
            return (Home,Away);
        }

        // Chat-GPT algo (idk how good it is)
        public static (List<PlayerMMR>, List<PlayerMMR>) DividePlayerMMRList(List<PlayerMMR> playerMMRs)
        {
            (List<PlayerMMR>, List<PlayerMMR>) bestPartition = (new List<PlayerMMR>(), new List<PlayerMMR>());
            int bestDiff = int.MaxValue;
            PartitionPlayerMMRList(playerMMRs, new List<PlayerMMR>(), new List<PlayerMMR>(), 0, 0, 0, Math.Min(5,(int)(playerMMRs.Count/2)), ref bestPartition, ref bestDiff);

            return bestPartition;
        }

        private static void PartitionPlayerMMRList(
            List<PlayerMMR> remainingPlayers,
            List<PlayerMMR> list1,
            List<PlayerMMR> list2,
            int mmr1,
            int mmr2,
            int index,
            int minTeamSize,
            ref (List<PlayerMMR>, List<PlayerMMR>) bestPartition,
            ref int bestDiff)
        {
            if (list1.Count >= minTeamSize && list1.Count <= 5 && list2.Count >= minTeamSize && list2.Count <= 5)
            {
                int diff = Math.Abs(mmr1 - mmr2);
                if (diff < bestDiff)
                {
                    bestDiff = diff;
                    bestPartition = (new List<PlayerMMR>(list1), new List<PlayerMMR>(list2));
                }
            }

            if (index >= remainingPlayers.Count || list1.Count > 5 || list2.Count > 5) return;

            PlayerMMR player = remainingPlayers[index];
            list1.Add(player);
            PartitionPlayerMMRList(remainingPlayers, list1, list2, mmr1 + (int)player.MMR, mmr2, index + 1, minTeamSize, ref bestPartition, ref bestDiff);
            list1.RemoveAt(list1.Count - 1);

            list2.Add(player);
            PartitionPlayerMMRList(remainingPlayers, list1, list2, mmr1, mmr2 + (int)player.MMR, index + 1, minTeamSize, ref bestPartition, ref bestDiff);
            list2.RemoveAt(list2.Count - 1);
        }

    }
}