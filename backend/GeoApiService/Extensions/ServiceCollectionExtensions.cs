using GeoApiService.Configuration;
using GeoApiService.Service;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace GeoApiService.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGeoapifyApi(this IServiceCollection services, HttpConfiguration configuration)
    {
        services.AddRefitClient<IGeoapifyApi>()
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = configuration.Uri;
                c.Timeout = configuration.Timeout;
            });

        services.AddTransient<IGeoapifyService, GeoapifyService>();

        return services;;
    }
}