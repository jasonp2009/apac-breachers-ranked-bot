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
    public class MatchConfirmedHandler : INotificationHandler<AllPlayersConfirmedEvent>
    {
        private readonly IDiscordClient _discordClient;
        private readonly IDbContext _dbContext;
        public MatchConfirmedHandler(IDiscordClient discordClient, IDbContext dbContext)
        {
            _discordClient = discordClient;
            _dbContext = dbContext;
        }

        public async Task Handle(AllPlayersConfirmedEvent notification, CancellationToken cancellationToken)
        {
            MatchEntity match = await _dbContext.Matches.FirstAsync(match => match.Id == notification.MatchId, cancellationToken);

            match.ConfirmMatch();

            MatchThreads matchThreads = await _dbContext.MatchThreads.FirstAsync(threads => threads.Match.Id == match.Id, cancellationToken);

            if (await _discordClient.GetChannelAsync(matchThreads.MatchThreadId) is not IThreadChannel channel) return;

            await channel.SendMessageAsync(embed: match.GenerateMatchConfirmedEmbed());

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
