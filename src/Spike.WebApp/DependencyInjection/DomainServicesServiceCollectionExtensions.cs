using Microsoft.EntityFrameworkCore;
using Spike.Common.Services;
using Spike.Domain.Commands;
using Spike.Domain.Services;
using Spike.Messaging.SqlServer.Services;
using Spike.SqlServer;
using Spike.SqlServer.Services;

namespace Spike.WebApp.DependencyInjection
{
    public static class DomainServicesServiceCollectionExtensions
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<SpikeDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("database")));

            services.AddSingleton(new SqlServerMessageOutboxOptions { SchemaName = SpikeDbContext .SchemaName});
            services.AddScoped<IUnitOfWork>(s => s.GetRequiredService<SpikeDbContext>());
            services.AddTransient<CreatePersonHandler>();
            services.AddTransient<IPersonRepository, SqlServerPersonRepository>();

            return services;
        }
    }
}
