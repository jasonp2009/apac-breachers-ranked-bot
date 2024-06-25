using ApacBreachersRanked.Application.Users;
using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.Match.Enums;
using ApacBreachersRanked.Domain.MatchData.Entities;
using ApacBreachersRanked.Domain.MatchData.Events;
using ApacBreachersRanked.Infrastructure.Breachers.Entities;
using ApacBreachersRanked.Infrastructure.Breachers.Events;
using ApacBreachersRanked.Infrastructure.Breachers.Models;
using ApacBreachersRanked.Infrastructure.Persistance;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApacBreachersRanked.Infrastructure.Breachers.EventHandlers;

internal class BreachersMatchDataReadyHandler : INotificationHandler<BreachersMatchDataReadyEvent>
{
    private readonly BreachersDbContext _breachersDbContext;
    private readonly IMediator _mediator;
    private readonly ILogger<BreachersMatchDataReadyHandler> _logger;

    public BreachersMatchDataReadyHandler(BreachersDbContext breachersDbContext, IMediator mediator, ILogger<BreachersMatchDataReadyHandler> logger)
    {
        _breachersDbContext = breachersDbContext;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Handle(BreachersMatchDataReadyEvent notification, CancellationToken cancellationToken)
    {
        BreachersMatchDataEntity breachersMatchData =
            await _breachersDbContext.BreachersMatchData
                .FirstOrDefaultAsync(x => x.Id == notification.MatchId,
                cancellationToken);
        MatchEntity match =
            await _breachersDbContext.Matches
                .Include(match => match.AllPlayers)
                .FirstOrDefaultAsync(x => x.Id == notification.MatchId, cancellationToken);
        List<BreachersDiscordUserLink> userLinks =
            await _breachersDbContext.BreachersDiscordUserLinks
                .Where(link => match.AllPlayers
                    .Select(player => player.UserId.GetDiscordId()).Contains(link.DiscordUserId))
                .ToListAsync(cancellationToken);
        foreach (GetMatchResponse breachersGame in breachersMatchData.Games)
        {
            List<GamePlayerData> matchPlayerData = new();
            foreach (BreachersPlayer breachersPlayer in breachersGame.GameData.AllPlayers)
            {
                BreachersDiscordUserLink userLink =
                    userLinks.FirstOrDefault(link => link.BreachersUserId == breachersPlayer.Id);
                if (userLink == null)
                {
                    _logger.LogWarning(
                        "Unable to find BreachersDiscordUserLink for {BreachersUserId} when processing breachers api data for {MatchId}",
                        breachersPlayer.Id,
                        notification.MatchId);
                    continue;
                }
                MatchPlayer matchPlayer =
                    match.AllPlayers.FirstOrDefault(x => x.UserId.GetDiscordId() == userLink.DiscordUserId);
                if (matchPlayer == null)
                {
                    _logger.LogWarning(
                        "Unable to find MatchPlayer for {BreachersUserId}/{DiscordUserId} when processing breachers api data for {MatchId}",
                        userLink.BreachersUserId,
                        userLink.DiscordUserId,
                        notification.MatchId);
                    continue;
                }

                List<GamePlayerRoundData> roundData = new();
                
                foreach (BreachersRound breachersRound in breachersPlayer.Rounds)
                {
                    List<WeaponData> weaponData = breachersRound.Weapons
                        .Select(breachersWeapon => new WeaponData
                        {
                            Type = breachersWeapon.Type,
                            Damage = breachersWeapon.DamageDone,
                            FriendlyDamage = breachersWeapon.FriendlyDamageDone,
                            HeadshotKills = breachersWeapon.HeadshotKills,
                            Headshots = breachersWeapon.TotalHeadshots,
                            Hits = breachersWeapon.TotalShotsHit,
                            Kills = breachersWeapon.TotalKills,
                            ShotsFired = breachersWeapon.ShotsFired
                        }).ToList();
                    List<GadgetData> gadgetData = breachersRound.Gadgets
                        .Select(breachersGadget => new GadgetData
                        {
                            Type = breachersGadget.Type,
                            Damage = breachersGadget.DamageDone,
                            Destroyed = breachersGadget.Destroyed,
                            EnemyTriggered = breachersGadget.EnemyTriggered,
                            FriendlyDamage = breachersGadget.FriendlyDamageDone,
                            Healed = breachersGadget.TeamHealed,
                            Kills = breachersGadget.Kills,
                            Triggered = breachersGadget.Triggered,
                            Used = breachersGadget.Used
                        }).ToList();
                    roundData.Add(new()
                    {
                        UserId = matchPlayer.UserId,
                        Name = matchPlayer.Name,
                        Ace = breachersRound.Ace,
                        Assists = breachersRound.Assists,
                        Died = breachersRound.Deaths > 0,
                        FirstBlood = breachersRound.FirstBlood,
                        Mvp = breachersRound.Mvp,
                        Won = breachersRound.Team == breachersRound.TeamWon,
                        RoundNumber = breachersRound.RoundNumber,
                        Weapons = weaponData,
                        Gadgets = gadgetData,
                        RoundTime = TimeSpan.FromSeconds(Convert.ToDouble(breachersRound.RoundTime))
                    });
                }

                matchPlayerData.Add(new()
                {
                    UserId = matchPlayer.UserId,
                    Name = matchPlayer.Name,
                    Mmr = matchPlayer.MMR,
                    Rank = matchPlayer.Rank,
                    Side = matchPlayer.Side,
                    Mvp = breachersPlayer.Mvp,
                    GameTime = breachersPlayer.GameTime,
                    Rounds = roundData
                });
            }
            
            int homeScore = matchPlayerData
                .Where(player => player.Side == MatchSide.Home)
                .MaxBy(player => player.GameTime).Rounds
                .Count(round => round.Won);
            int awayScore = matchPlayerData
                .Where(player => player.Side == MatchSide.Away)
                .MaxBy(player => player.GameTime).Rounds
                .Count(round => round.Won);
            MapScore score = new(breachersGame.GameData.Map, homeScore, awayScore);
            GameDataEntity gameData = new()
            {
                MatchId = match.Id,
                Match = match,
                Score = score,
                Players = matchPlayerData,
            };
            gameData.QueueDomainEvent(new GameDataReadyEvent { MatchId = gameData.MatchId });
            _breachersDbContext.Add(gameData);
            await _breachersDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
