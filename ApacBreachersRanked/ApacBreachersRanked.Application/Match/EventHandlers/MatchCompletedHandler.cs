﻿using ApacBreachersRanked.Application.Config;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.Match.Extensions;
using ApacBreachersRanked.Application.Match.Models;
using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.MMR.Entities;
using ApacBreachersRanked.Domain.MMR.Events;
using Discord;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ApacBreachersRanked.Application.Match.EventHandlers
{
    public class MatchCompletedHandler : INotificationHandler<MatchMMRCalculatedEvent>
    {
        private readonly IDbContext _dbContext;
        private readonly IDiscordClient _discordClient;
        private readonly BreachersDiscordOptions _options;
        
        public MatchCompletedHandler(IDbContext dbContext, IDiscordClient discordClient, IOptions<BreachersDiscordOptions> options)
        {
            _dbContext = dbContext;
            _discordClient = discordClient;
            _options = options.Value;
        }
        public async Task Handle(MatchMMRCalculatedEvent notification, CancellationToken cancellationToken)
        {
            MatchEntity match = await _dbContext.Matches.AsNoTracking()
                .Include(x => x.AllPlayers)
                .Include(x => x.Score)
                .Where(x => x.Id == notification.MatchId)
                .SingleAsync(cancellationToken);
            MatchThreads matchThreads = await _dbContext.MatchThreads.AsNoTracking()
                .Where(x => x.Match.Id == notification.MatchId)
                .SingleAsync(cancellationToken);
            IQueryable<MMRAdjustment> mmrAdjustments = _dbContext.MMRAdjustments.AsNoTracking().Where(x => x.Match.Id == notification.MatchId);

            if (await _discordClient.GetChannelAsync(_options.MatchResultsChannelId) is IMessageChannel matchResultsChannel)
            {
                await matchResultsChannel.SendMessageAsync(embed: match.GenerateMatchResultEmbed(mmrAdjustments));
            }

            if (await _discordClient.GetChannelAsync(matchThreads.MatchThreadId) is IThreadChannel threadChannel)
            {
                await threadChannel.SendMessageAsync(embed: match.GenerateMatchResultEmbed(mmrAdjustments));
                await threadChannel.ModifyAsync(chnl => {
                    chnl.Archived = true;
                    chnl.Locked = true;
                });
            }
        }
    }
}
