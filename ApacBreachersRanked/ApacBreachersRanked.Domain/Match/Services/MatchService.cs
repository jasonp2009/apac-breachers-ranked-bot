using ApacBreachersRanked.Domain.Helpers;
using ApacBreachersRanked.Domain.Match.Constants;
using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.Match.Interfaces;
using ApacBreachersRanked.Domain.MatchQueue.Entities;
using ApacBreachersRanked.Domain.User.Interfaces;

namespace ApacBreachersRanked.Domain.Match.Services
{
    internal class MatchService : IMatchService
    {
        private readonly IUserService _userService;

        public MatchService(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<MatchEntity> CreateMatchFromQueue(MatchQueueEntity matchQueue)
        {
            IEnumerable<Task<IUser>> userTasks = matchQueue.Users
                .Take(MatchConstants.MaxCapacity)
                .Select(x => _userService.GetUserAsync(x.UserId));

            await Task.WhenAll(userTasks);

            List<IUser> users = userTasks.Select(userTask => userTask.Result).ToList();

            (List<IUser> home, List<IUser> away) = RandomiseTeams(users);

            return new MatchEntity(matchQueue, home, away);

        }

        public (List<IUser> Home, List<IUser> Away) RandomiseTeams(List<IUser> users)
        {
            users.Shuffle();

            List<IUser> home = new(), away = new();

            bool selectHome = true;

            foreach (var user in users)
            {
                if (selectHome)
                {
                    home.Add(user);
                }
                else
                {
                    away.Add(user);
                }
                selectHome = !selectHome;
            }
            return (home, away);
        }
    }
}
