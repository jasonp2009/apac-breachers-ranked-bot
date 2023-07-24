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
    public class PlayerConfirmedHandler : INotificationHandler<PlayerConfirmedEvent>
    {
        private readonly IDbContext _dbContext;
        private readonly IDiscordClient _discordClient;

        public PlayerConfirmedHandler(IDbContext dbContext, IDiscordClient discordClient)
        {
            _dbContext = dbContext;
            _discordClient = discordClient;
        }

        public async Task Handle(PlayerConfirmedEvent notification, CancellationToken cancellationToken)
        {
            MatchEntity match = await _dbContext.Matches.SingleAsync(match => match.Id == notification.MatchId, cancellationToken);
            MatchThreads? matchThreads = await _dbContext.MatchThreads.FirstOrDefaultAsync(matchThreads => matchThreads.Match.Id == notification.MatchId, cancellationToken);
            if (matchThreads == null) return;

            IChannel channel = await _discordClient.GetChannelAsync(matchThreads.MatchThreadId);
            if (channel is IThreadChannel threadChannel)
            {
                IMessage message = await threadChannel.GetMessageAsync(matchThreads.MatchThreadWelcomeMessageId);
                if (message is IUserMessage userMessage)
                {
                    await userMessage.ModifyAsync(msg => msg.Embed = match.GenerateMatchEmbed());
                }
            }
        }
    }
}
