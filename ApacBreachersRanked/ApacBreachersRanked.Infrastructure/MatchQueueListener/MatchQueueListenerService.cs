using ApacBreachersRanked.Application.Match.Commands;
using ApacBreachersRanked.Domain.Match.Constants;
using ApacBreachersRanked.Infrastructure.Persistance;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ApacBreachersRanked.Infrastructure.MatchQueueListener
{
    public class MatchQueueListenerService : IHostedService, IDisposable
    {
        private Timer? _timer = null;
        private readonly IServiceProvider _services;
        private readonly ILogger<MatchQueueListenerService> _logger;
        private CancellationToken _stoppingToken;

        private bool IsForceStartEnabled = false;

        public MatchQueueListenerService(IServiceProvider services, ILogger<MatchQueueListenerService> logger)
        {
            _services = services;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _stoppingToken = stoppingToken;
            _timer = new Timer(
                async _ => await DoWorkAsync(),
                null,
                TimeSpan.FromSeconds(30),
                TimeSpan.FromSeconds(30));

            return Task.CompletedTask;
        }

        private async Task DoWorkAsync()
        {
            using (IServiceScope scope = _services.CreateScope())
            {
                ILogger<MatchQueueListenerService> logger = scope.ServiceProvider.GetRequiredService<ILogger<MatchQueueListenerService>>();
                BreachersDbContext dbContext = scope.ServiceProvider.GetRequiredService<BreachersDbContext>();
                IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                try
                {
                    if (IsForceStartEnabled)
                    {
                        if (await dbContext.MatchQueue.AnyAsync(x => x.IsOpen && x.Users.Count >= MatchConstants.MinCapacity))
                        {
                            await mediator.Send(new CreateMatchCommand(), _stoppingToken);
                        }
                        IsForceStartEnabled = false;
                    } else
                    {
                        if (await dbContext.MatchQueue.AnyAsync(x => x.IsOpen && x.Users.Count >= MatchConstants.MaxCapacity))
                        {
                            await mediator.Send(new CreateMatchCommand(), _stoppingToken);
                        }
                    } 
                    
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occured when attempting to create a match");
                }
            }
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public void ForceStart()
        {
            IsForceStartEnabled = true;
        }
    }
}
