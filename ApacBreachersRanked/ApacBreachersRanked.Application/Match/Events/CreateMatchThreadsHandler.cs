using ApacBreachersRanked.Application.Config;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.Match.Models;
using ApacBreachersRanked.Application.Users;
using ApacBreachersRanked.Domain.Entities;
using Discord;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApacBreachersRanked.Application.Match.Events
{
    public class CreateMatchThreadsHandler : INotificationHandler<MatchCreatedEvent>
    {
        private readonly IDbContext _dbContext;
        private readonly IDiscordClient _discordClient;
        private readonly BreachersDiscordOptions _breachersDiscordOptions;

        public CreateMatchThreadsHandler(
            IDbContext dbContext,
            IDiscordClient discordClient,
            IOptions<BreachersDiscordOptions> breachersDiscordOptions)
        {
            _dbContext = dbContext;
            _discordClient = discordClient;
            _breachersDiscordOptions = breachersDiscordOptions.Value;
        }

        public async Task Handle(MatchCreatedEvent notification, CancellationToken cancellationToken)
        {
            MatchEntity? match = await _dbContext.Matches.FirstOrDefaultAsync(x => x.Id == notification.MatchId, cancellationToken);

            if (match == null) return;

            if (await _dbContext.MatchThreads.AnyAsync(x => x.Match.Id == notification.MatchId, cancellationToken)) return;

            MatchThreads matchThreads = await CreateMatchThreads(match);
            _dbContext.MatchThreads.Add(matchThreads);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private async Task<MatchThreads> CreateMatchThreads(MatchEntity match)
        {
            MatchThreads matchThreads = new(match);
            ITextChannel matchChannel = await _discordClient.GetChannelAsync(_breachersDiscordOptions.MatchChannelId) as ITextChannel;
            var matchThreadTask = CreateThreadWithPlayers(matchChannel, "Match", "@here Welcome to the match", match.AllPlayers);
            var homeThreadTask = CreateThreadWithPlayers(matchChannel, "Home", "@here This is a thread for just your team", match.HomePlayers);
            var awayThreadTask = CreateThreadWithPlayers(matchChannel, "Away", "@here This is a thread for just your team", match.AwayPlayers);

            await Task.WhenAll(matchThreadTask, homeThreadTask, awayThreadTask);

            matchThreads.MatchThreadId = matchThreadTask.Result.Id;
            matchThreads.HomeThreadId = homeThreadTask.Result.Id;
            matchThreads.AwayThreadId = awayThreadTask.Result.Id;
            return matchThreads;
        }

        private async Task<IThreadChannel> CreateThreadWithPlayers(ITextChannel matchChannel, string threadName, string welcomeMessage, IEnumerable<MatchPlayer> players)
        {
            IThreadChannel thread = await matchChannel.CreateThreadAsync(threadName, ThreadType.PrivateThread, ThreadArchiveDuration.OneDay);
            await Task.WhenAll(
                players.Select(player => InviteUserToThread(thread, player)));

            await thread.SendMessageAsync(welcomeMessage);
            return thread;
        }

        private async Task InviteUserToThread(IThreadChannel threadChannel, MatchPlayer user)
        {
            if (user.UserId is ApplicationDiscordUserId userId)
            {
                IGuildUser? guildUser = await _discordClient.GetUserAsync(userId) as IGuildUser;
                if (guildUser != null)
                {
                    await threadChannel.AddUserAsync(guildUser);
                }
            }
        }
    }
}
