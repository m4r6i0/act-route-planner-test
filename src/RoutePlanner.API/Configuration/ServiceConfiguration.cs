using Microsoft.Extensions.DependencyInjection;
using RoutePlanner.API.Repositories;
using RoutePlanner.API.Services;

namespace RoutePlanner.API.Configurations
{
    public static class ServiceConfiguration
    {
        public static void ConfigureDependencies(this IServiceCollection services, string routesFilePath)
        {
            services.AddSingleton(new RouteRepository(routesFilePath));
            services.AddScoped<RouteService>();
        }
    }
}
