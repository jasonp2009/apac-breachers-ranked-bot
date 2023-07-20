using Discord;
using MediatR;

namespace ApacBreachersRanked.Application.Users
{
    public class GetDiscordUserQuery : IRequest<DiscordUser>
    {
        public ulong DiscordUserId { get; set; }
    }

    public class GetDiscordUserQueryHandler : IRequestHandler<GetDiscordUserQuery, DiscordUser>
    {
        private readonly IDiscordUserRepository _userRepository;
        private readonly IDiscordClient _discordClient;

        public GetDiscordUserQueryHandler(
            IDiscordUserRepository userRepository,
            IDiscordClient discordClient)
        {
            _userRepository = userRepository;
            _discordClient = discordClient;
        }

        public async Task<DiscordUser> Handle(GetDiscordUserQuery request, CancellationToken cancellationToken)
        {
            DiscordUser? discordUser = _userRepository.Query.FirstOrDefault(user => user.DiscordUserId == request.DiscordUserId);
            if (discordUser == null)
            {
                IGuild guild = await _discordClient.GetGuildAsync(123);
                IGuildUser guildUser = await guild.GetUserAsync(request.DiscordUserId)
                    ?? throw new InvalidOperationException("Cannot find user in server");
                discordUser = new()
                {
                    DiscordUserId = guildUser.Id
                };
                await _userRepository.SaveAsync(discordUser, cancellationToken);
            }
            return discordUser;
        }
    }
}
