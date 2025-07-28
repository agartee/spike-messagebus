using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Spike.Messaging.Services;
using Spike.Messaging.SqlServer.Services;
using Spike.SqlServer.Services;

namespace Spike.Messaging.SqlServer.DependencyInjection
{
    public static class MessagingServiceCollectionExtensions
    {
        public static IServiceCollection AddSqlServerMessageOutbox<TDbContext>(this IServiceCollection services, SqlServerMessageOutboxOptions options)
            where TDbContext : DbContext, IMessageOutbox
        {
            services.AddTransient<IMessageOutboxWriter, SqlServerMessageOutboxWriter<TDbContext>>();
            services.AddTransient<IMessageOutboxReader, SqlServerMessageOutboxReader<TDbContext>>();
            services.AddSingleton(options);

            return services;
        }
    }
}
