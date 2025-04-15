using Microsoft.Extensions.DependencyInjection;

namespace Feature.Countries;

public static class CountryFeatureInstaller
{
    public static IServiceCollection AddCountryFeature(this IServiceCollection services)
    {
        return services;
    }
}
