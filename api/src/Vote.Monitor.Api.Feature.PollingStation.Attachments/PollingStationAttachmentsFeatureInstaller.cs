using Microsoft.Extensions.DependencyInjection;

namespace Vote.Monitor.Api.Feature.PollingStation.Attachments;

public static class PollingStationAttachmentsFeatureInstaller
{
    public static IServiceCollection AddPollingStationAttachmentsFeature(this IServiceCollection services)
    {
        return services;
    }
}
