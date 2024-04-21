using Microsoft.Extensions.DependencyInjection;

namespace Feature.Attachments;

public static class AttachmentsFeatureInstaller
{
    public static IServiceCollection AddAttachmentsFeature(this IServiceCollection services)
    {
        return services;
    }
}
