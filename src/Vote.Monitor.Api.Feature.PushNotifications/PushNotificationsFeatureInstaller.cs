using Microsoft.Extensions.DependencyInjection;

namespace Vote.Monitor.Api.Feature.PushNotifications;

public static class PushNotificationsFeatureInstaller
{
    public static IServiceCollection AddPushNotificationsFeature(this IServiceCollection services)
    {
        return services;
    }
}
