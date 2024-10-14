using Microsoft.Extensions.DependencyInjection;

namespace Feature.Citizen.Notifications;

public static class CitizenNotificationsFeatureInstaller
{
    public static IServiceCollection AddCitizenNotificationsFeature(this IServiceCollection services)
    {
        return services;
    }
}
