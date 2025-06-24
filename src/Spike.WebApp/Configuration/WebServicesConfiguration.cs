using Azure.Messaging.ServiceBus;
using Spike.WebApp.Services;

namespace Spike.WebApp.Configuration
{
    public static class WebServicesConfiguration
    {
        public static IServiceCollection AddWebServices(this IServiceCollection services, IConfiguration configuration)
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
