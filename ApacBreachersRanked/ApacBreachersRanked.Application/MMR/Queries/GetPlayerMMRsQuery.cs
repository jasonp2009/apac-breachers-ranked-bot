using ApacBreachersRanked.Application.Common.Mediator;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Domain.Match.Enums;
using ApacBreachersRanked.Domain.MMR.Entities;
using ApacBreachersRanked.Domain.User.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Application.MMR.Queries;

public class GetPlayerMMRsQuery : IQuery<List<PlayerMMR>>
{
    public IEnumerable<IUser> Users { get; set; }
    public MatchFormat MatchFormat { get; set; }
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

        foreach (var user in request.Users)
        {
            var playerMMR = await _dbContext.PlayerMMRs
                .Where(x => x.UserId.Equals(user.UserId) && x.MatchFormat == request.MatchFormat)
                .FirstOrDefaultAsync(cancellationToken);

            if (playerMMR == null)
            {
                playerMMR = new PlayerMMR(user, request.MatchFormat);
                await _dbContext.PlayerMMRs.AddAsync(playerMMR, cancellationToken);
            }

            playerMMRs.Add(playerMMR);
        }

        return playerMMRs;
    }
}