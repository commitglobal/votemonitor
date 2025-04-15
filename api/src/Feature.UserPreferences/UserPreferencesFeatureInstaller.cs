using Microsoft.Extensions.DependencyInjection;

namespace Feature.UserPreferences;

public static class UserPreferencesFeatureInstaller
{
    public static IServiceCollection AddUserPreferencesFeature(this IServiceCollection services)
    {
        return services;
    }
}
