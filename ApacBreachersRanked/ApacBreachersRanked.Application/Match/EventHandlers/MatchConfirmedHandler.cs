﻿using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.Match.Extensions;
using ApacBreachersRanked.Application.Match.Models;
using ApacBreachersRanked.Application.MatchVote.Events;
using ApacBreachersRanked.Application.MatchVote.Models;
using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.Match.Events;
using Discord;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApacBreachersRanked.Application.Match.Events
{
    public class MatchConfirmedHandler : INotificationHandler<SideVoteCompletedEvent>
    {
        private readonly IDiscordClient _discordClient;
        private readonly IDbContext _dbContext;
        private readonly ILogger<MatchConfirmedHandler> _logger;
        public MatchConfirmedHandler(IDiscordClient discordClient, IDbContext dbContext, ILogger<MatchConfirmedHandler> logger)
        {
            _discordClient = discordClient;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Handle(SideVoteCompletedEvent notification, CancellationToken cancellationToken)
        {
            MatchEntity match = await _dbContext.Matches
                .AsNoTracking()
                .FirstAsync(match => match.Id == notification.MatchId, cancellationToken);

            MatchVoteModel matchVote = await _dbContext.MatchVotes
                .AsNoTracking()
                .SingleAsync(x => x.MatchId ==  notification.MatchId, cancellationToken);

            MatchThreads matchThreads = await _dbContext.MatchThreads
                .AsNoTracking()
                .FirstAsync(threads => threads.Match.Id == match.Id, cancellationToken);

            if (await _discordClient.GetChannelAsync(matchThreads.MatchThreadId) is IThreadChannel channel)
            {
                if (await channel.GetMessageAsync(matchThreads.MatchThreadWelcomeMessageId) is IUserMessage message)
                {
                    try
                    {
                        await message.ModifyAsync(msg => {
                            msg.Components = new ComponentBuilder().Build();
                            msg.Embed = match.GenerateMatchWelcomeEmbed();
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "An error occurred when trying to update the match welcome message for match {MatchId}", notification.MatchId);
                    }
                    
                }
                await channel.SendMessageAsync(embed: match.GenerateMatchConfirmedEmbed(matchVote));
            }
        }
    }
}
