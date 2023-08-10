using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.Match.Extensions;
using ApacBreachersRanked.Application.Match.Models;
using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.Match.Events;
using Discord;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Application.Match.Events
{
    public class MatchConfirmedHandler : INotificationHandler<MatchConfirmedEvent>
    {
        private readonly IDiscordClient _discordClient;
        private readonly IDbContext _dbContext;
        public MatchConfirmedHandler(IDiscordClient discordClient, IDbContext dbContext)
        {
            _discordClient = discordClient;
            _dbContext = dbContext;
        }

        public async Task Handle(MatchConfirmedEvent notification, CancellationToken cancellationToken)
        {
            MatchEntity match = await _dbContext.Matches.FirstAsync(match => match.Id == notification.MatchId, cancellationToken);

            MatchThreads matchThreads = await _dbContext.MatchThreads.FirstAsync(threads => threads.Match.Id == match.Id, cancellationToken);

            if (await _discordClient.GetChannelAsync(matchThreads.MatchThreadId) is IThreadChannel channel)
            {
                if (await channel.GetMessageAsync(matchThreads.MatchThreadWelcomeMessageId) is IUserMessage message)
                {
                    await message.ModifyAsync(msg => msg.Components = new ComponentBuilder().Build());
                }
                await channel.SendMessageAsync(embed: match.GenerateMatchConfirmedEmbed());
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
