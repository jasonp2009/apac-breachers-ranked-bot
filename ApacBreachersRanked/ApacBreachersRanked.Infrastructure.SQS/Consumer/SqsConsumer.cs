using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using ApacBreachersRanked.Infrastructure.SQS.Extensions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ApacBreachersRanked.Infrastructure.SQS.Consumer
{
    internal class SqsConsumer : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly SqsOptions _config;
        private readonly IAmazonSQS _sqsClient;
        private readonly ILogger _logger;
        private CancellationToken _stoppingToken;

        public SqsConsumer(IServiceProvider services, IOptions<SqsOptions> config, ILogger<SqsConsumer> logger)
        {
            _services = services;
            _config = config.Value;
            BasicAWSCredentials basicCredentials = new BasicAWSCredentials(_config.AccessKey, _config.Secret);
            RegionEndpoint region = RegionEndpoint.GetBySystemName(_config.Region);
            _sqsClient = new AmazonSQSClient(basicCredentials, region);
            _logger = logger;
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
            try
            {
                _logger.LogInformation("Begin processing {MessageId} with body: {@Body}",
                    message.MessageId,
                    message.Body);
                
                INotification? notification = MessageSerializer.Deserialize<INotification>(message.Body);
                if (notification != null)
                {
                    using (IServiceScope scope = _services.CreateScope())
                    {
                        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                        await mediator.Publish(notification, _stoppingToken);
                    }
                }
                await _sqsClient.DeleteMessageAsync(_config.QueueUrl, message.ReceiptHandle, _stoppingToken);
                
                _logger.LogInformation("Successfully processed {MessageId} with body: {@Body}",
                    message.MessageId,
                    message.Body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "An exception occurred when trying to process {MessageId} with body: {@Body}",
                    message.MessageId,
                    message.Body);
            }
        }
    }
}
