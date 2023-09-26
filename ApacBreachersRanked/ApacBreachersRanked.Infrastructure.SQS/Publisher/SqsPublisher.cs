using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using ApacBreachersRanked.Domain.Common;
using ApacBreachersRanked.Infrastructure.SQS.Extensions;
using MediatR;
using Microsoft.Extensions.Options;

namespace ApacBreachersRanked.Infrastructure.SQS.Publisher
{
    internal class SqsPublisher : INotificationPublisher
    {
        private readonly SqsOptions _config;
        private readonly IAmazonSQS _sqsClient;
        public SqsPublisher(IOptions<SqsOptions> config)
        {
            _config = config.Value;
            BasicAWSCredentials basicCredentials = new BasicAWSCredentials(_config.AccessKey, _config.Secret);
            RegionEndpoint region = RegionEndpoint.GetBySystemName(_config.Region);
            _sqsClient = new AmazonSQSClient(basicCredentials, region);
        }

        public Task Publish(IEnumerable<NotificationHandlerExecutor> handlerExecutors, INotification notification, CancellationToken cancellationToken)
        {
            if (notification is IScheduledEvent scheduledEvent && DateTime.UtcNow < scheduledEvent.ScheduledForUtc)
            {
                SendMessageRequest request = new()
                {
                    QueueUrl = _config.QueueUrl,
                    DelaySeconds = scheduledEvent.GetSqsDelay(),
                    MessageBody = MessageSerializer.Serialize(notification)
                };
                return _sqsClient.SendMessageAsync(request, cancellationToken);
            }
            return _sqsClient.SendMessageAsync(_config.QueueUrl, MessageSerializer.Serialize(notification), cancellationToken);
        }
    }
}
