using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.Match.Models;
using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.Match.Events;
using Discord;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Application.Match.EventHandlers
{
    public class MatchCancelledHandler : INotificationHandler<MatchCancelledEvent>
    {
        private readonly IDbContext _dbContext;
        private readonly IDiscordClient _discordClient;

        public MatchCancelledHandler(IDbContext dbContext, IDiscordClient discordClient)
        {
            _dbContext = dbContext;
            _discordClient = discordClient;
        }

        public async Task Handle(MatchCancelledEvent notification, CancellationToken cancellationToken)
        {
            MatchEntity match = await _dbContext.Matches.AsNoTracking().SingleAsync(x => x.Id == notification.MatchId, cancellationToken);
            MatchThreads matchThreads = await _dbContext.MatchThreads.AsNoTracking().SingleAsync(x => x.Match.Id == notification.MatchId, cancellationToken);

            if (await _discordClient.GetChannelAsync(matchThreads.MatchThreadId) is IThreadChannel channel)
            {
                await channel.SendMessageAsync($"Cancelling match{Environment.NewLine}" +
                                               $"{match.CancellationReason}");
                await channel.ModifyAsync(chnl => chnl.Archived = true);
            }
        }
    }
}
