using ApacBreachersRanked.Application.Common.Mediator;
using ApacBreachersRanked.Application.Config;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.MMR.Commands;
using ApacBreachersRanked.Application.MMR.Extensions;
using ApacBreachersRanked.Application.MMR.Models;
using ApacBreachersRanked.Application.Stats.Models;
using ApacBreachersRanked.Application.Stats.Queries;
using ApacBreachersRanked.Application.Users;
using ApacBreachersRanked.Domain.MMR.Entities;
using ApacBreachersRanked.Domain.MMR.Events;
using Discord;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ApacBreachersRanked.Application.MMR.EventHandlers
{
    public class UpdateLeaderBoardHandler : INotificationHandler<MatchMMRCalculatedEvent>, ICommandHandler<RefreshLeaderboardCommand>
    {
        private readonly IDbContext _dbContext;
        private readonly IMediator _mediator;
        private readonly IDiscordClient _discordClient;
        private readonly BreachersDiscordOptions _config;

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
        public async Task Handle(MatchMMRCalculatedEvent notification, CancellationToken cancellationToken)
        {
            List<PlayerMMR> top50Players = await _dbContext.PlayerMMRs
                .OrderByDescending(x => x.MMR)
                .Take(50)
                .ToListAsync(cancellationToken);

            List<LeaderBoardPlayer> leaderBoardPlayers = new();

            foreach(PlayerMMR playerMMR in top50Players)
            {
                BasicPlayerStats basicStats = await _mediator.Send(new GetBasicPlayerStatsQuery { DiscordUserId = playerMMR.UserId.GetDiscordId() }, cancellationToken);
                leaderBoardPlayers.Add(new(playerMMR, basicStats.Match));
            }

            LeaderBoardMessage? leaderBoardMessage = await _dbContext.LeaderBoardMessages.FirstOrDefaultAsync(cancellationToken);

            if (leaderBoardMessage == null)
            {
                leaderBoardMessage = new();
                await _dbContext.LeaderBoardMessages.AddAsync(leaderBoardMessage);
            }

            if (await _discordClient.GetChannelAsync(_config.LeaderBoardChannelId) is ITextChannel channel)
            {
                if (leaderBoardMessage.LeaderBoardMessageId != 0 &&
                    await channel.GetMessageAsync(leaderBoardMessage.LeaderBoardMessageId) is IUserMessage message)
                {
                    await message.ModifyAsync(msg => msg.Embed = leaderBoardPlayers.GetLeaderBoardEmbed());
                } else
                {
                    IUserMessage newMessage = await channel.SendMessageAsync(embed: leaderBoardPlayers.GetLeaderBoardEmbed());
                    leaderBoardMessage.LeaderBoardMessageId = newMessage.Id;
                    await _dbContext.SaveChangesAsync(cancellationToken);
                }
            }
        }

        public async Task<Unit> Handle(RefreshLeaderboardCommand request, CancellationToken cancellationToken)
        {
            await Handle(new MatchMMRCalculatedEvent(), cancellationToken);
            return Unit.Value;
        }
    }
}
