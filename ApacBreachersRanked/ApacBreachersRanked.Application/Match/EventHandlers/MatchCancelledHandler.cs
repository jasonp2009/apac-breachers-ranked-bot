using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Domain.Match.Events;
using Discord;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Application.Match.EventHandlers;

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
        var match = await _dbContext.Matches.AsNoTracking()
            .Where(x => x.Id == notification.MatchId)
            .SingleOrDefaultAsync(cancellationToken);
        var matchThreads = await _dbContext.MatchThreads.AsNoTracking()
            .Where(x => x.Match.Id == notification.MatchId)
            .SingleOrDefaultAsync(cancellationToken);

        if (match is null || matchThreads is null) return;

        if (await _discordClient.GetChannelAsync(matchThreads.MatchThreadId) is IThreadChannel channel)
        {
            await channel.SendMessageAsync($"Cancelling match{Environment.NewLine}" +
                                           $"{match.CancellationReason}");
            if (await channel.GetMessageAsync(matchThreads.MatchThreadWelcomeMessageId) is IUserMessage message)
                await message.ModifyAsync(msg => msg.Components = new ComponentBuilder().Build());
            await channel.ModifyAsync(chnl =>
            {
                chnl.Archived = true;
                chnl.Locked = true;
            });
        }
    }
}