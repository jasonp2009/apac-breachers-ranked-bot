using ApacBreachersRanked.Domain.Common;
using ApacBreachersRanked.Infrastructure.Persistance;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ApacBreachersRanked.Infrastructure.ScheduledEventHandling
{
    internal class ScheduledEventHandlingService : BackgroundService
    {
        private PeriodicTimer? _timer = null;
        private readonly IServiceProvider _services;
        private readonly ILogger<ScheduledEventHandlingService> _logger;
        private CancellationToken _stoppingToken;

        public ScheduledEventHandlingService(IServiceProvider services, ILogger<ScheduledEventHandlingService> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _timer = new (TimeSpan.FromSeconds(5));
            _stoppingToken = stoppingToken;

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

                List<ScheduledEvent> events = await dbContext.ScheduledEvents.Where(x => x.ScheduledForUtc <= DateTime.UtcNow).ToListAsync(_stoppingToken);

                if (events.Count == 0) return;

                foreach (var scheduledEvent in events)
                {
                    if (scheduledEvent.ScheduledForUtc <= DateTime.UtcNow)
                    {
                        try
                        {
                            if (scheduledEvent.Event != null)
                            {
                                await HandleEvent(scheduledEvent.Event);
                            }

                            dbContext.ScheduledEvents.Remove(scheduledEvent);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "An exception occurred when trying to DoWorkAsync()");
                        }
                    }
                }
                await dbContext.SaveChangesAsync(_stoppingToken);
            }
        }

        private async Task HandleEvent(IScheduledEvent scheduledEvent)
        {
            using (IServiceScope scope = _services.CreateScope())
            {
                try
                {
                    IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    await mediator.Publish(scheduledEvent, _stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An exception occurent when trying to HandleEvent()");
                }
            }
        }

        public void ScheduleEvent(IDomainEvent domainEvent)
        {
            _ = Task.Run(async () =>
            {
                using (IServiceScope scope = _services.CreateScope())
                {
                    try
                    {
                        if (domainEvent is IScheduledEvent scheduledEvent)
                        {
                            BreachersDbContext dbContext = scope.ServiceProvider.GetRequiredService<BreachersDbContext>();
                            await dbContext.AddAsync(new ScheduledEvent(scheduledEvent), _stoppingToken);
                            await dbContext.SaveChangesAsync(_stoppingToken);
                        }
                        else
                        {
                            IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                            await mediator.Publish(domainEvent, _stoppingToken);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "An exception occurent when trying to ScheduleEvent()");
                    }
                }
            });
        }
    }
}
