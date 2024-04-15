using Microsoft.Extensions.DependencyInjection;

namespace Vote.Monitor.Api.Feature.Emergencies.Attachments;

public static class EmergencyAttachmentsFeatureInstaller
{
    public static IServiceCollection AddEmergencyAttachmentsFeature(this IServiceCollection services)
    {
        return services;
    }
}
