using Microsoft.Extensions.DependencyInjection;

namespace Vote.Monitor.Api.Feature.Country;

public static class CountryFeatureInstaller
{
    public static IServiceCollection AddCountryFeature(this IServiceCollection services)
    {
        return services;
    }
}
