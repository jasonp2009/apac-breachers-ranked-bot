using MediatR;

namespace ApacBreachersRanked.Application.PingTimer.Events
{
    public class RolePingedEvent : INotification
    {
        public ulong RoleId { get; set; }
    }
}
