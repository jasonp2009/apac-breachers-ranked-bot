using ApacBreachersRanked.Domain.Common;

namespace ApacBreachersRanked.Infrastructure.SQS.Extensions
{
    internal static class IScheduledEventExtensions
    {
        public static int GetSqsDelay(this IScheduledEvent scheduledEvent)
        {
            TimeSpan delay = scheduledEvent.ScheduledForUtc - DateTime.UtcNow;
            int delaySeconds;
            if (delay.Minutes > 15)
            {
                delaySeconds = 15 * 60;
            }
            else
            {
                delaySeconds = delay.Seconds;
            }
            return delaySeconds;
        }
    }
}
