using ApacBreachersRanked.Infrastructure.SQS.Consumer;
using ApacBreachersRanked.Infrastructure.SQS.Publisher;
using MediatR;
using MediatR.NotificationPublishers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ApacBreachersRanked.Infrastructure.SQS
{
    public static class DependencyInjection
    {
        public static IServiceCollection UseSqsPublisher(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SqsOptions>(options => configuration.GetSection(SqsOptions.Key).Bind(options));
            services.AddSingleton<INotificationPublisher,SqsPublisher>();
            services.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());

                cfg.NotificationPublisherType = typeof(SqsPublisher);
            });
            return services;
        }

        public static IServiceCollection UseSqsConsumer(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SqsOptions>(options => configuration.GetSection(SqsOptions.Key).Bind(options));

            services.AddSingleton<SqsPublisher>();
            services.AddSingleton<SqsConsumer>();

            services.AddHostedService(serviceProvider => serviceProvider.GetRequiredService<SqsConsumer>());

            services.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());

                cfg.NotificationPublisherType = typeof(ConsumerNotificationPublisher);
            });

            return services;
        }
    }
}
