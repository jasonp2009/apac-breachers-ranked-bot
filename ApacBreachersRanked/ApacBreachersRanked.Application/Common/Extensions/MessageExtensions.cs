using ApacBreachersRanked.Application.Users;
using ApacBreachersRanked.Domain.User.Interfaces;

namespace ApacBreachersRanked.Application.Common.Extensions
{
    public static class MessageExtensions
    {
        public static string GetUserMention(this IUser user)
            => $"<@{user.UserId.GetDiscordId()}>";
    }
}
