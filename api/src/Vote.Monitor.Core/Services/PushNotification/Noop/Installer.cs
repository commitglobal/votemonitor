using Microsoft.Extensions.DependencyInjection;
using Vote.Monitor.Core.Services.PushNotification.Contracts;

namespace Vote.Monitor.Core.Services.PushNotification.Noop;

internal static class Installer
{
    internal static IServiceCollection AddNoopNotificationSender(this IServiceCollection services)
    {
        services.AddSingleton<IPushNotificationService, NoopPushNotificationService>();

        return services;
    }
}
