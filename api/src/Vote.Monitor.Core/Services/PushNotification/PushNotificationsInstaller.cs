using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vote.Monitor.Core.Services.PushNotification.Firebase;
using Vote.Monitor.Core.Services.PushNotification.Noop;

namespace Vote.Monitor.Core.Services.PushNotification;

public static class PushNotificationsInstaller
{
    public const string SectionKey = "PushNotifications";
    public static IServiceCollection AddPushNotifications(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        switch (configuration.GetValue<PushNotificationSenderType>("SenderType"))
        {
            case PushNotificationSenderType.Noop:
                return serviceCollection.AddNoopNotificationSender();

            case PushNotificationSenderType.Firebase:
                var firebaseSection = configuration.GetSection(FirebaseOptions.SectionName);
                return serviceCollection.AddFirebaseNotificationSender(firebaseSection);

            default:
                throw new ArgumentException("Unknown configuration for SenderType", nameof(configuration));
        }
    }
}
