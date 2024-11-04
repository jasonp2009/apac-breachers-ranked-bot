using ApacBreachersRanked.Application.Common.Mediator;
using ApacBreachersRanked.Application.Config;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.MMR.Commands;
using ApacBreachersRanked.Application.MMR.Extensions;
using ApacBreachersRanked.Application.MMR.Models;
using ApacBreachersRanked.Application.Stats.Queries;
using ApacBreachersRanked.Application.Users;
using ApacBreachersRanked.Domain.Helpers;
using ApacBreachersRanked.Domain.Match.Enums;
using ApacBreachersRanked.Domain.MMR.Events;
using Discord;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ApacBreachersRanked.Application.MMR.EventHandlers;

public class UpdateLeaderBoardHandler : INotificationHandler<MatchMMRCalculatedEvent>,
    ICommandHandler<RefreshLeaderboardCommand>
{
    private readonly BreachersDiscordOptions _config;
    private readonly IDbContext _dbContext;
    private readonly IDiscordClient _discordClient;
    private readonly IMediator _mediator;

    public UpdateLeaderBoardHandler(
        IDbContext dbContext,
        IMediator mediator,
        IDiscordClient discordClient,
        IOptions<BreachersDiscordOptions> options)
    {
        _dbContext = dbContext;
        _mediator = mediator;
        _discordClient = discordClient;
        _config = options.Value;
    }

    public async Task<Unit> Handle(RefreshLeaderboardCommand request, CancellationToken cancellationToken)
    {
        await Handle(new MatchMMRCalculatedEvent(), cancellationToken);
        return Unit.Value;
    }

    public async Task Handle(MatchMMRCalculatedEvent notification, CancellationToken cancellationToken)
    {
        Dictionary<MatchFormat, List<LeaderBoardPlayer>> matchFormatLeaderBoardPlayers = new();
        foreach (var matchFormat in MatchConstantsExtensions.GetEnabledMatchFormats())
        {
            var top50Players = await _dbContext.PlayerMMRs
                .Where(x => x.MatchFormat == matchFormat && x.Rank != null)
                .OrderByDescending(x => x.MMR)
                .Take(50)
                .ToListAsync(cancellationToken);

            List<LeaderBoardPlayer> leaderBoardPlayers = new();

            foreach (var playerMMR in top50Players)
            {
                var basicStats = await _mediator.Send(
                    new GetBasicPlayerStatsQuery
                        { DiscordUserId = playerMMR.UserId.GetDiscordId(), MatchFormat = matchFormat },
                    cancellationToken);
                leaderBoardPlayers.Add(new LeaderBoardPlayer(playerMMR, basicStats.Match));
            }

            matchFormatLeaderBoardPlayers.Add(matchFormat, leaderBoardPlayers);
        }

        var embeds = matchFormatLeaderBoardPlayers.Select(leaderBoardPlayers =>
            leaderBoardPlayers.Value.GetLeaderBoardEmbed(leaderBoardPlayers.Key)).ToArray();

        var leaderBoardMessage =
            await _dbContext.LeaderBoardMessages.FirstOrDefaultAsync(cancellationToken);

        if (leaderBoardMessage == null)
        {
            leaderBoardMessage = new LeaderBoardMessage();
            await _dbContext.LeaderBoardMessages.AddAsync(leaderBoardMessage, cancellationToken);
        }

        if (await _discordClient.GetChannelAsync(_config.LeaderBoardChannelId) is ITextChannel channel)
        {
            if (leaderBoardMessage.LeaderBoardMessageId != 0 &&
                await channel.GetMessageAsync(leaderBoardMessage.LeaderBoardMessageId) is IUserMessage message)
            {
                await message.ModifyAsync(msg => msg.Embeds = embeds);
            }
            else
            {
                var newMessage = await channel.SendMessageAsync(embeds: embeds);
                leaderBoardMessage.LeaderBoardMessageId = newMessage.Id;
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}