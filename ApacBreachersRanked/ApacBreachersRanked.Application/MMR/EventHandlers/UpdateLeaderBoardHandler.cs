using ApacBreachersRanked.Application.Config;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.MMR.Extensions;
using ApacBreachersRanked.Application.MMR.Models;
using ApacBreachersRanked.Domain.MMR.Entities;
using ApacBreachersRanked.Domain.MMR.Events;
using Discord;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ApacBreachersRanked.Application.MMR.EventHandlers
{
    public class UpdateLeaderBoardHandler : INotificationHandler<MatchMMRCalculatedEvent>
    {
        private readonly IDbContext _dbContext;
        private readonly IDiscordClient _discordClient;
        private readonly BreachersDiscordOptions _config;

        public UpdateLeaderBoardHandler(
            IDbContext dbContext,
            IDiscordClient discordClient,
            IOptions<BreachersDiscordOptions> options)
        {
            _dbContext = dbContext;
            _discordClient = discordClient;
            _config = options.Value;
        }
        public async Task Handle(MatchMMRCalculatedEvent notification, CancellationToken cancellationToken)
        {
            List<PlayerMMR> top50Players = await _dbContext.PlayerMMRs
                .OrderByDescending(x => x.MMR)
                .Take(50)
                .ToListAsync(cancellationToken);

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
                    await message.ModifyAsync(msg => msg.Embed = top50Players.GetLeaderBoardEmbed());
                } else
                {
                    IUserMessage newMessage = await channel.SendMessageAsync(embed: top50Players.GetLeaderBoardEmbed());
                    leaderBoardMessage.LeaderBoardMessageId = newMessage.Id;
                    await _dbContext.SaveChangesAsync(cancellationToken);
                }
            }
        }
    }
}
