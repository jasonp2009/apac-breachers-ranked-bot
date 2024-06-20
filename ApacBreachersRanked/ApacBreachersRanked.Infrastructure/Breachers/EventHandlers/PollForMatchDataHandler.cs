using ApacBreachersRanked.Application.Users;
using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Infrastructure.Breachers.Api;
using ApacBreachersRanked.Infrastructure.Breachers.Entities;
using ApacBreachersRanked.Infrastructure.Breachers.Events;
using ApacBreachersRanked.Infrastructure.Breachers.Models;
using ApacBreachersRanked.Infrastructure.Persistance;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Infrastructure.Breachers.EventHandlers;

internal class PollForMatchDataHandler : INotificationHandler<PollForMatchDataEvent>
{
    private readonly BreachersDbContext _dbContext;
    private readonly BreachersApiClient _breachersApiClient;
    private readonly IMediator _mediator;

    public PollForMatchDataHandler(BreachersDbContext dbContext, BreachersApiClient breachersApiClient, IMediator mediator)
    {
        _dbContext = dbContext;
        _breachersApiClient = breachersApiClient;
        _mediator = mediator;
    }

    public async Task Handle(PollForMatchDataEvent notification, CancellationToken cancellationToken)
    {
        MatchEntity match = await _dbContext.Matches.FirstOrDefaultAsync(x => x.Id == notification.MatchId, cancellationToken)
                            ?? throw new KeyNotFoundException($"Invalid match id: {notification.MatchId}");
        List<string> homeUserIds = await _dbContext.BreachersDiscordUserLinks
            .Where(link => match.HomePlayers.Select(player => player.UserId.GetDiscordId()).Contains(link.DiscordUserId))
            .Select(link => link.BreachersUserId)
            .ToListAsync(cancellationToken);
        List<string> awayUserIds = await _dbContext.BreachersDiscordUserLinks
            .Where(link => match.AwayPlayers.Select(player => player.UserId.GetDiscordId()).Contains(link.DiscordUserId))
            .Select(link => link.BreachersUserId)
            .ToListAsync(cancellationToken);
        List<string> allBreachersIds = homeUserIds.Concat(awayUserIds).ToList();
        Random rnd = new();
        string userIdToCheck = allBreachersIds[rnd.Next(allBreachersIds.Count)];
        IEnumerable<GetMatchResponse> games = await _breachersApiClient.GetMatchesByUserId(userIdToCheck, cancellationToken);
        List<GetMatchResponse> matchingGames = games.Where(game =>
        {
            List<string> teamA = game.GameData.AllPlayers
                .Where(player =>
                    player.Rounds.Any(round => round.RoundNumber == 1 && round.Team == BreachersSide.Revolters))
                .Select(x => x.Id).ToList();
            List<string> teamB = game.GameData.AllPlayers
                .Where(player =>
                    player.Rounds.Any(round => round.RoundNumber == 1 && round.Team == BreachersSide.Enforcers))
                .Select(x => x.Id).ToList();
            
            return game.GameData.AllPlayers.All(player => { return allBreachersIds.Contains(player.Id); }) &&
                   ((teamA.All(id => homeUserIds.Contains(id)) && teamB.All(id => awayUserIds.Contains(id))) ||
                   (teamA.All(id => awayUserIds.Contains(id)) && teamB.All(id => homeUserIds.Contains(id))));
        }).ToList();
        if (matchingGames.Count == 0)
        {
            await _mediator.Publish(new PollForMatchDataEvent()
            {
                MatchId = notification.MatchId,
                ScheduledForUtc = DateTime.UtcNow + TimeSpan.FromSeconds(30)
            });
            return;
        }

        MatchStatEntity matchStatEntity = new()
        {
            Id = notification.MatchId,
            Games = matchingGames
        };
        _dbContext.MatchStats.Add(matchStatEntity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
