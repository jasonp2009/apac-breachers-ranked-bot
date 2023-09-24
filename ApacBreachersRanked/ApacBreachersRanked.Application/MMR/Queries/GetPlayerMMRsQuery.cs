using ApacBreachersRanked.Application.Common.Mediator;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Domain.MMR.Entities;
using ApacBreachersRanked.Domain.User.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Application.MMR.Queries
{
    public class GetPlayerMMRsQuery : IQuery<List<PlayerMMR>>
    {
        public IEnumerable<IUser> Users { get; set; }
    }

    public class GetPlayerMMRsQueryHandler : IQueryHandler<GetPlayerMMRsQuery, List<PlayerMMR>>
    {
        private readonly IDbContext _dbContext;

        public GetPlayerMMRsQueryHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<PlayerMMR>> Handle(GetPlayerMMRsQuery request, CancellationToken cancellationToken)
        {
            List<PlayerMMR> playerMMRs = new();

            foreach (IUser user in request.Users)
            {
                PlayerMMR? playerMMR = await _dbContext.PlayerMMRs
                    .Where(x => x.UserId.Equals(user.UserId))
                    .FirstOrDefaultAsync(cancellationToken);

                if (playerMMR == null)
                {
                    playerMMR = new(user);
                    await _dbContext.PlayerMMRs.AddAsync(playerMMR, cancellationToken);
                }
                playerMMRs.Add(playerMMR);
            }

            return playerMMRs;
        }
    }
}
