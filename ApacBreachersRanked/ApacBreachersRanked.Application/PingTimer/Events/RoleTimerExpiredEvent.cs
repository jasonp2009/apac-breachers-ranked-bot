using MediatR;

namespace ApacBreachersRanked.Application.PingTimer.Events
{
    public class RoleTimerExpiredEvent : INotification
    {
        public ulong RoleId { get; set; }
    }
}
