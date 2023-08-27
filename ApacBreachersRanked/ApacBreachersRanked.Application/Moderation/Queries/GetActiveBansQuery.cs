using ApacBreachersRanked.Application.Common.Mediator;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.Moderation.Models;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Application.Moderation.Queries
{
    public class GetActiveBansQuery : IQuery<IQueryable<UserBan>>
    {
    }

    public class GetActiveBansHandler : IQueryHandler<GetActiveBansQuery, IQueryable<UserBan>>
    {
        private readonly IDbContext _dbcontext;

        public GetActiveBansHandler(IDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }
        public async Task<IQueryable<UserBan>> Handle(GetActiveBansQuery request, CancellationToken cancellationToken)
            => _dbcontext.UserBans
                .AsNoTracking()
                .Where(x => x.BannedAtUtc <= DateTime.UtcNow && DateTime.UtcNow <= x.ExpiryUtc && !x.UnBanOverride);
    }
}
