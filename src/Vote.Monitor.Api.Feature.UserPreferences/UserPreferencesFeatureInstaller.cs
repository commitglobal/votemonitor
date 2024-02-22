using Microsoft.Extensions.DependencyInjection;

namespace Vote.Monitor.Api.Feature.UserPreferences;

public static class UserPreferencesFeatureInstaller
{
    public static IServiceCollection AddUserPreferencesFeature(this IServiceCollection services)
    {
        return services;
    }
}
