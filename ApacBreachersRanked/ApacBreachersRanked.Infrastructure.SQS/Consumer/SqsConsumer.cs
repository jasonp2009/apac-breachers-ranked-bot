using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using ApacBreachersRanked.Domain.Common;
using ApacBreachersRanked.Infrastructure.SQS.Extensions;
using ApacBreachersRanked.Infrastructure.SQS.Publisher;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
namespace ApacBreachersRanked.Infrastructure.SQS.Consumer
{
    internal class SqsConsumer : BackgroundService
    {
        private readonly SqsPublisher _sqsPublisher;
        private readonly IServiceProvider _services;
        private readonly SqsOptions _config;
        private readonly IAmazonSQS _sqsClient;
        private CancellationToken _stoppingToken;

        public SqsConsumer(IServiceProvider services, IOptions<SqsOptions> config)
        {
            _services = services;
            _config = config.Value;
            BasicAWSCredentials basicCredentials = new BasicAWSCredentials(_config.AccessKey, _config.Secret);
            RegionEndpoint region = RegionEndpoint.GetBySystemName(_config.Region);
            _sqsClient = new AmazonSQSClient(basicCredentials, region);
            _sqsPublisher = new(config);
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _stoppingToken = stoppingToken;
            while (!_stoppingToken.IsCancellationRequested)
            {
                ReceiveMessageResponse messageResponse = await _sqsClient.ReceiveMessageAsync(new ReceiveMessageRequest
                {
                    QueueUrl = _config.QueueUrl,
                    MaxNumberOfMessages = _config.MaxMessages,
                    WaitTimeSeconds = _config.WaitTime
                });

                await Task.WhenAll(messageResponse.Messages.Select(HandleMessage));
            }
        }

        private async Task HandleMessage(Message message)
        {
            INotification? notification = MessageSerializer.Deserialize<INotification>(message.Body);
            if (notification == null) return;
            if (notification is IScheduledEvent scheduledEvent && DateTime.UtcNow < scheduledEvent.ScheduledForUtc)
            {
                await _sqsPublisher.Publish(new List<NotificationHandlerExecutor>(), notification, _stoppingToken);
                return;
            }
            using (IServiceScope scope = _services.CreateScope())
            {
                IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                await mediator.Publish(notification, _stoppingToken);
            }
            await _sqsClient.DeleteMessageAsync(_config.QueueUrl, message.ReceiptHandle, _stoppingToken);
        }
    }
}
