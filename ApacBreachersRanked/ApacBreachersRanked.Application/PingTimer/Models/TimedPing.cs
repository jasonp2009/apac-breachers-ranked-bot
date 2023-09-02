namespace ApacBreachersRanked.Application.PingTimer.Models
{
    public class TimedPing
    {
        public ulong RoleId { get; private set; }
        public int TimeOutMins { get; private set; }
        public bool IsTimedOut { get; private set; }
        public DateTime NextPingUtc { get; private set; }

        public void TimeOut()
        {
            IsTimedOut = true;
            NextPingUtc = DateTime.UtcNow + TimeSpan.FromMinutes(TimeOutMins);
        }

        public void ReleaseTimeOut()
        {
            IsTimedOut = false;
        }
    }
}
