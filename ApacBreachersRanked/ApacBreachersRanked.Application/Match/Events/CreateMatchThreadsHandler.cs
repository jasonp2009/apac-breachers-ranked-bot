using ApacBreachersRanked.Application.Config;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.Match.Models;
using ApacBreachersRanked.Application.Users;
using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.Match.Events;
using ApacBreachersRanked.Application.Match.Extensions;
using Discord;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

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
            (IThreadChannel matchThread, IUserMessage message) = await CreateThreadWithPlayers(matchChannel, $"Match #{match.MatchNumber}", "@here", match.GenerateMatchEmbed(), match.AllPlayers);

            matchThreads.MatchThreadId = matchThread.Id;
            matchThreads.MatchThreadWelcomeMessageId = message.Id;
            return matchThreads;

            /*
            var homeThreadTask = CreateThreadWithPlayers(matchChannel, $"Match #{match.MatchNumber}: Home", "@here", match.GenerateHomeEmbed(), match.HomePlayers);
            var awayThreadTask = CreateThreadWithPlayers(matchChannel, $"Match #{match.MatchNumber}: Away", "@here", match.GenerateAwayEmbed(), match.AwayPlayers);

            await Task.WhenAll(matchThreadTask, homeThreadTask, awayThreadTask);

            matchThreads.MatchThreadId = matchThreadTask.Result.Id;
            matchThreads.HomeThreadId = homeThreadTask.Result.Id;
            matchThreads.AwayThreadId = awayThreadTask.Result.Id;
            return matchThreads;
            */
        }

        private async Task<(IThreadChannel thread, IUserMessage message)> CreateThreadWithPlayers(ITextChannel matchChannel, string threadName, string welcomeMessage, Embed embed, IEnumerable<MatchPlayer> players)
        {
            IThreadChannel thread = await matchChannel.CreateThreadAsync(threadName, ThreadType.PrivateThread, ThreadArchiveDuration.OneDay);
            await Task.WhenAll(
                players.Select(player => InviteUserToThread(thread, player)));
            IUserMessage message = await thread.SendMessageAsync(welcomeMessage, embed: embed);
            return (thread, message);
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
