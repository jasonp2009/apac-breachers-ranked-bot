using ApacBreachersRanked.Application.Config;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.Match.Models;
using ApacBreachersRanked.Application.Users;
using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.Match.Events;
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
            var matchThreadTask = CreateThreadWithPlayers(matchChannel, $"Match #{match.MatchNumber}", "@here", GenerateMatchEmbed(match), match.AllPlayers);
            var homeThreadTask = CreateThreadWithPlayers(matchChannel, $"Match #{match.MatchNumber}: Home", "@here", GenerateHomeEmbed(match), match.HomePlayers);
            var awayThreadTask = CreateThreadWithPlayers(matchChannel, $"Match #{match.MatchNumber}: Away", "@here", GenerateAwayEmbed(match), match.AwayPlayers);

            await Task.WhenAll(matchThreadTask, homeThreadTask, awayThreadTask);

            matchThreads.MatchThreadId = matchThreadTask.Result.Id;
            matchThreads.HomeThreadId = homeThreadTask.Result.Id;
            matchThreads.AwayThreadId = awayThreadTask.Result.Id;
            return matchThreads;
        }

        private async Task<IThreadChannel> CreateThreadWithPlayers(ITextChannel matchChannel, string threadName, string welcomeMessage, Embed embed, IEnumerable<MatchPlayer> players)
        {
            IThreadChannel thread = await matchChannel.CreateThreadAsync(threadName, ThreadType.PrivateThread, ThreadArchiveDuration.OneDay);
            await Task.WhenAll(
                players.Select(player => InviteUserToThread(thread, player)));

            await thread.SendMessageAsync(welcomeMessage, embed: embed);
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

        private Embed GenerateMatchEmbed(MatchEntity match)
        {
            EmbedBuilder eb = new();
            eb.WithTitle("Welcome to the match");
            eb.WithDescription(
                $"The teams are:{Environment.NewLine}" +
                $"Home:{Environment.NewLine}" +
                string.Join(Environment.NewLine, match.HomePlayers.Select(homePlayer => $"    <@{homePlayer.UserId.GetDiscordId()}>")) +
                $"{Environment.NewLine}vs{Environment.NewLine}" +
                string.Join(Environment.NewLine, match.AwayPlayers.Select(awayPlayer => $"    <@{awayPlayer.UserId.GetDiscordId()}>"))
            );
            return eb.Build();
        }

        private Embed GenerateHomeEmbed(MatchEntity match)
        {
            EmbedBuilder eb = new();
            eb.WithTitle("Welcome to the match Home team");
            eb.WithDescription(
                $"This is a thread for just your team{Environment.NewLine}" +
                $"You team is:{Environment.NewLine}" +
                string.Join(Environment.NewLine, match.HomePlayers.Select(homePlayer => $"    <@{homePlayer.UserId.GetDiscordId()}>"))
            );
            return eb.Build();
        }

        private Embed GenerateAwayEmbed(MatchEntity match)
        {
            EmbedBuilder eb = new();
            eb.WithTitle("Welcome to the match Away team");
            eb.WithDescription(
                $"This is a thread for just your team{Environment.NewLine}" +
                $"You team is:{Environment.NewLine}" +
                string.Join(Environment.NewLine, match.AwayPlayers.Select(awayPlayer => $"    <@{awayPlayer.UserId.GetDiscordId()}>"))
            );
            return eb.Build();
        }
    }
}
