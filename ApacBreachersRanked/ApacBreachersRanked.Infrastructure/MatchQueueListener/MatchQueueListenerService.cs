using ApacBreachersRanked.Application.Match.Commands;
using ApacBreachersRanked.Domain.Match.Constants;
using ApacBreachersRanked.Domain.MatchQueue.Entities;
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
        private PeriodicTimer? _timer = null;
        private readonly IServiceProvider _services;
        private readonly ILogger<MatchQueueListenerService> _logger;
        private CancellationToken _stoppingToken;

        private bool IsForceStartEnabled = false;

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
                ILogger<MatchQueueListenerService> logger = scope.ServiceProvider.GetRequiredService<ILogger<MatchQueueListenerService>>();
                BreachersDbContext dbContext = scope.ServiceProvider.GetRequiredService<BreachersDbContext>();
                IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                try
                {
                    MatchQueueEntity? currentQueue = await dbContext.MatchQueue.FirstOrDefaultAsync(x => x.IsOpen, _stoppingToken);
                    if (IsForceStartEnabled || (currentQueue?.Users.All(x => x.VoteToForce) ?? false))
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

        public void ForceStart()
        {
            IsForceStartEnabled = true;
        }
    }
}
