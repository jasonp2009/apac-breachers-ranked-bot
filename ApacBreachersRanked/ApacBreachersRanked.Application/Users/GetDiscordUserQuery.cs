using ApacBreachersRanked.Application.Config;
using Discord;
using MediatR;
using Microsoft.Extensions.Options;

namespace ApacBreachersRanked.Application.Users
{
    public class GetDiscordUserQuery : IRequest<ApplicationDiscordUser>
    {
        public ulong DiscordUserId { get; set; }
    }

    public class GetDiscordUserQueryHandler : IRequestHandler<GetDiscordUserQuery, ApplicationDiscordUser>
    {
        private readonly IDiscordClient _discordClient;
        private readonly BreachersDiscordOptions _breachersDiscordOptions;

        public GetDiscordUserQueryHandler(
            IDiscordClient discordClient,
            IOptions<BreachersDiscordOptions> beachersDiscordOptions)
        {
            _discordClient = discordClient;
            _breachersDiscordOptions = beachersDiscordOptions.Value;
        }

        public async Task<ApplicationDiscordUser> Handle(GetDiscordUserQuery request, CancellationToken cancellationToken)
        {
            IGuild guild = await _discordClient.GetGuildAsync(_breachersDiscordOptions.GuildId);
            IGuildUser guildUser = await guild.GetUserAsync(request.DiscordUserId)
                ?? throw new InvalidOperationException("Cannot find user in server");
            return new ApplicationDiscordUser(guildUser);
        }
    }
}
