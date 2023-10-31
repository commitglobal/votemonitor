using Microsoft.Extensions.DependencyInjection;

namespace Vote.Monitor.Country;

public static class CountryFeatureInstaller
{
    public static IServiceCollection AddCountryFeature(this IServiceCollection services)
    {
        return services;
    }
}
