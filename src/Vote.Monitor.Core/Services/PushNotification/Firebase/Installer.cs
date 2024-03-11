using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vote.Monitor.Core.Services.PushNotification.Contracts;

namespace Vote.Monitor.Core.Services.PushNotification.Firebase;

internal static class Installer
{
    internal static IServiceCollection AddFirebaseNotificationSender(this IServiceCollection services, IConfiguration configuration)
    {
        var firebaseOptions = new FirebaseOptions();
        configuration.Bind(firebaseOptions);

        services.AddSingleton(firebaseOptions);

        services.Configure<FirebaseOptions>(configuration);

        FirebaseApp.Create(new AppOptions
        {
            Credential = GoogleCredential.FromJson(firebaseOptions.Token)
        });

        services.AddSingleton<IPushNotificationService, FirebasePushNotificationService>();

        return services;
    }
}
