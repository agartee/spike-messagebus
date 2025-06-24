using Spike.WebApp.Services;

namespace Spike.WebApp.Configuration
{
    public static class WebServicesConfiguration
    {
        public static IServiceCollection AddWebServices(this IServiceCollection services)
        {
            services.AddHostedService<OutboxDispatcherService>();
            services.AddScoped<IOutboxDispatchWorker, OutboxDispatchWorker>();

            return services;
        }
    }
}
