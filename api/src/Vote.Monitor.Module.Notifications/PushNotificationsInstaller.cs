using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vote.Monitor.Module.Notifications.Expo;
using Vote.Monitor.Module.Notifications.Firebase;
using Vote.Monitor.Module.Notifications.Noop;

namespace Vote.Monitor.Module.Notifications;

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

            case PushNotificationSenderType.Expo:
                var expoSection = configuration.GetSection(ExpoOptions.SectionName);
                return serviceCollection.AddExpoNotificationSender(expoSection);

            default:
                throw new ArgumentException("Unknown configuration for SenderType", nameof(configuration));
        }
    }
}
