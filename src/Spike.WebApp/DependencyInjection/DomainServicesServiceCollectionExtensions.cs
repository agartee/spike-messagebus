using Microsoft.EntityFrameworkCore;
using Spike.Domain.Commands;
using Spike.Domain.Services;
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

            services.AddTransient<CreatePersonHandler>();
            services.AddTransient<CreatePlaceHandler>();

            services.AddTransient<IPersonRepository, SqlServerPersonRepository>();
            services.AddTransient<IPlaceRepository, SqlServerPlaceRepository>();

            return services;
        }
    }
}
