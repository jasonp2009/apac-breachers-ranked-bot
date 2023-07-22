
using ApacBreachersRanked.Domain.Interfaces;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ApacBreachersRanked.Application.Users
{
    public class ApplicationDiscordUserId : IUserId
    {
        public ulong Id { get; private set; }

        public ApplicationDiscordUserId(ulong id)
        {
            Id = id;
        }

        public static implicit operator ulong(ApplicationDiscordUserId user)
        {
            return user.Id;
        }

        public bool Equals(IUserId rhs)
        {
            if (rhs is ApplicationDiscordUserId rhsTyped)
            {
                return Id == rhsTyped.Id;
            }
            return false;
        }
    }

    public class ApplicationDiscordUserIdValueConvertor : ValueConverter<IUserId, ulong>
    {
        public ApplicationDiscordUserIdValueConvertor() : base(
            userId => ConvertToUlong(userId),
            v => new ApplicationDiscordUserId(v)
            )
        { }

        private static ulong ConvertToUlong(IUserId userId)
        {
            if (userId is ApplicationDiscordUserId applicationDiscordUserId)
            {
                return applicationDiscordUserId.Id;
            }
            else
            {
                throw new InvalidOperationException($"Unsupport implementation of IUserId: {userId.GetType()}");
            }
        }
    }

    public class ApplicationDiscordUser : IUser
    {
        private Discord.IUser _discordUser;
        public IUserId UserId => new ApplicationDiscordUserId(_discordUser.Id);
        public string Name => _discordUser.GlobalName;

        public ApplicationDiscordUser(Discord.IUser discordUser)
        {
            _discordUser = discordUser;
        }
    }
}
