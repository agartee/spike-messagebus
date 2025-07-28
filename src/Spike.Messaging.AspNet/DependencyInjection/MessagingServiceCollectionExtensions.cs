using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spike.Messaging.AspNet.Services;

namespace Spike.Messaging.AspNet.DependencyInjection
{
    public static class MessagingServiceCollectionExtensions
    {
        public static IServiceCollection AddAspNetMessageDispatching(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHostedService<OutboxDispatcherService>();
            services.AddScoped<IOutboxDispatchWorker, OutboxDispatchWorker>();

            services.AddSingleton(sp =>
            {
                var connectionString = configuration["AzureServiceBus:ConnectionString"]
                    ?? throw new InvalidOperationException("Missing Azure Service Bus connection string.");
                return new ServiceBusClient(connectionString);
            });

            services.AddSingleton(sp =>
            {
                var client = sp.GetRequiredService<ServiceBusClient>();
                var queueName = configuration["AzureServiceBus:QueueName"]
                    ?? throw new InvalidOperationException("Missing Azure Service Bus queue name.");
                return client.CreateSender(queueName);
            });

            return services;
        }
    }
}
