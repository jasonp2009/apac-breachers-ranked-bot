using ApacBreachersRanked.Application.Common.Mediator;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.Users;
using ApacBreachersRanked.Application.Users.Services;
using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.Match.Enums;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Application.Stats.Queries;

public class GetMatchHistoryQuery : IQuery<IEnumerable<MatchEntity>>
{
    public int Limit { get; set; } = 10;
    public int Page { get; set; } = 0;
}

public class GetUserMatchHistoryQuery : GetMatchHistoryQuery, IQuery<IEnumerable<MatchEntity>>
{
    public ulong? DiscordUserId { get; set; }
}

public class GetMatchHistoryQueryHandler :
    IQueryHandler<GetMatchHistoryQuery, IEnumerable<MatchEntity>>,
    IQueryHandler<GetUserMatchHistoryQuery, IEnumerable<MatchEntity>>
{
    private readonly IDbContext _dbContext;
    private readonly DiscordUserContextService _userContextService;
    private readonly IMapper _mapper;

    public GetMatchHistoryQueryHandler(IDbContext dbContext, DiscordUserContextService userContextService, IMapper mapper)
    {
        _dbContext = dbContext;
        _userContextService = userContextService;
        _mapper = mapper;
    }

    public async Task<IEnumerable<MatchEntity>> Handle(GetMatchHistoryQuery request, CancellationToken cancellationToken)
    {
        List<MatchEntity> matches = await _dbContext.Matches
            .Include(x => x.AllPlayers)
            .Where(x => x.Status == MatchStatus.Completed)
            .OrderByDescending(x => x.MatchNumber)
            .Skip(request.Page * request.Limit)
            .Take(request.Limit)
            .ToListAsync(cancellationToken);
        return matches;
    }
    
    public async Task<IEnumerable<MatchEntity>> Handle(GetUserMatchHistoryQuery request, CancellationToken cancellationToken)
    {
        ulong discordUserId = request.DiscordUserId
                             ?? _userContextService.GetDiscordUser()?.UserId.GetDiscordId()
                             ?? throw new ArgumentNullException(nameof(GetUserMatchHistoryQuery.DiscordUserId));
        
        List<MatchEntity> matches = await _dbContext.Matches
            .Include(x => x.AllPlayers)
            .Where(x => x.Status == MatchStatus.Completed &&
                        x.AllPlayers.Any(p => p.UserId == discordUserId.ToIUserId()))
            .OrderByDescending(x => x.MatchNumber)
            .Skip(request.Page * request.Limit)
            .Take(request.Limit)
            .ToListAsync(cancellationToken);
        return matches;
    }
}
