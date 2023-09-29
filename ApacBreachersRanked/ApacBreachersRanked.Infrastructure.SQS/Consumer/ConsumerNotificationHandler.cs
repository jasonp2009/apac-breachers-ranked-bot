using ApacBreachersRanked.Domain.Common;
using ApacBreachersRanked.Infrastructure.SQS.Publisher;
using MediatR;

namespace ApacBreachersRanked.Infrastructure.SQS.Consumer
{
    internal class ConsumerNotificationPublisher : INotificationPublisher
    {
        private readonly SqsPublisher _sqsPublisher;
        public ConsumerNotificationPublisher(SqsPublisher sqsPublisher)
        {
            _sqsPublisher = sqsPublisher;
        }
        public async Task Publish(IEnumerable<NotificationHandlerExecutor> handlerExecutors, INotification notification, CancellationToken cancellationToken)
        {

            if (notification is IScheduledEvent scheduledEvent && DateTime.UtcNow < scheduledEvent.ScheduledForUtc)
            {
                await _sqsPublisher.Publish(new List<NotificationHandlerExecutor>(), notification, cancellationToken);
                return;
            }
            foreach (var handler in handlerExecutors)
            {
                await handler.HandlerCallback(notification, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
