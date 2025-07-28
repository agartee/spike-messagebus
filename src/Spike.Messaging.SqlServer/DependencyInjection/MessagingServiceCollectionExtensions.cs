using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Spike.Common.Models;
using Spike.Messaging.Services;
using Spike.Messaging.SqlServer.Services;
using Spike.SqlServer.Services;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Spike.Messaging.SqlServer.DependencyInjection
{
    public static class MessagingServiceCollectionExtensions
    {
        public static IServiceCollection AddSqlServerMessageOutbox<TDbContext>(this IServiceCollection services, 
            Action<SqlServerMessageOutboxOptions> configureOptions, Assembly? scanAssemblyForIdTypes = null) where TDbContext : DbContext, IMessageOutbox
        {
            services.AddTransient<IMessageOutboxWriter, SqlServerMessageOutboxWriter<TDbContext>>();
            services.AddTransient<IMessageOutboxReader, SqlServerMessageOutboxReader<TDbContext>>();

            var options = new SqlServerMessageOutboxOptions();
            
            if (scanAssemblyForIdTypes != null)
            {
                foreach (var idType in GetIdTypes(scanAssemblyForIdTypes))
                {
                    var converterType = typeof(StronglyTypedIdJsonConverter<>).MakeGenericType(idType);
                    var converter = (JsonConverter)Activator.CreateInstance(converterType)!;
                    options.JsonSerializerOptions.Converters.Add(converter);
                }
            }

            configureOptions(options);
            services.AddSingleton(options);

            return services;
        }

        private static IEnumerable<Type> GetIdTypes(Assembly scanAssembly)
        {
            return scanAssembly
                .GetTypes()
                .Where(t => t is { IsValueType: true, IsAbstract: false })
                .Where(t => typeof(IStronglyTypedId).IsAssignableFrom(t));
        }
    }
}
