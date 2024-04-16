using Microsoft.Extensions.DependencyInjection;
using Vote.Monitor.Module.Notifications.Contracts;

namespace Vote.Monitor.Module.Notifications.Noop;

internal static class Installer
{
    internal static IServiceCollection AddNoopNotificationSender(this IServiceCollection services)
    {
        services.AddSingleton<IPushNotificationService, NoopPushNotificationService>();

        return services;
    }
}
