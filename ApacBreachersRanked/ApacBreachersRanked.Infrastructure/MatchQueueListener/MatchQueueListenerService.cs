using ApacBreachersRanked.Application.Match.Commands;
using ApacBreachersRanked.Domain.Helpers;
using ApacBreachersRanked.Infrastructure.Persistance;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ApacBreachersRanked.Infrastructure.MatchQueueListener
{
    public class MatchQueueListenerService : BackgroundService
    {
        private PeriodicTimer _timer = null;
        private readonly IServiceProvider _services;
        private readonly ILogger<MatchQueueListenerService> _logger;
        private CancellationToken _stoppingToken;

        private bool _isForceStartEnabled = false;

        public MatchQueueListenerService(IServiceProvider services, ILogger<MatchQueueListenerService> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _stoppingToken = stoppingToken;
            _timer = new(TimeSpan.FromSeconds(10));
            while (await _timer.WaitForNextTickAsync(stoppingToken))
            {
                await DoWorkAsync();
            }
        }

        private async Task DoWorkAsync()
        {
            using (IServiceScope scope = _services.CreateScope())
            {
                BreachersDbContext dbContext = scope.ServiceProvider.GetRequiredService<BreachersDbContext>();
                IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                try
                {
                    var currentQueues = await dbContext.MatchQueue.Where(x => x.IsOpen)
                        .Include(x => x.Users)
                        .ToListAsync(_stoppingToken);
                    foreach (var currentQueue in currentQueues)
                    {
                        var matchConstants = currentQueue.MatchFormat.GetMatchFormatConstants();
                        if (_isForceStartEnabled || (currentQueue.Users.All(x => x.VoteToForce)))
                        {
                            if (await dbContext.MatchQueue.AnyAsync(x => x.IsOpen && x.Users.Count >= matchConstants.MinCapacity, cancellationToken: _stoppingToken))
                            {
                                await mediator.Send(new CreateMatchCommand { MatchFormat = currentQueue.MatchFormat}, _stoppingToken);
                            }
                            _isForceStartEnabled = false;
                            continue;
                        }

                        if (await dbContext.MatchQueue.AnyAsync(x => x.IsOpen && x.Users.Count >= matchConstants.MaxCapacity, cancellationToken: _stoppingToken))
                        {
                            await mediator.Send(new CreateMatchCommand { MatchFormat = currentQueue.MatchFormat}, _stoppingToken);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occured when attempting to create a match");
                }
            }
        }

        public void ForceStart()
        {
            _isForceStartEnabled = true;
        }
    }
}
