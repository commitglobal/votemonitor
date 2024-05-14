using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Vote.Monitor.Core.Converters;

namespace Feature.Notifications;

public static class PushNotificationsFeatureInstaller
{
    public static IServiceCollection AddPushNotificationsFeature(this IServiceCollection services)
    {

        SqlMapper.AddTypeHandler(typeof(NotificationReceiver[]), new JsonToObjectConverter<NotificationReceiver[]>());
        return services;
    }
}
