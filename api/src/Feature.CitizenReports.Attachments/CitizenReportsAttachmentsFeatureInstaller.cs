using Microsoft.Extensions.DependencyInjection;

namespace Feature.CitizenReports.Attachments;

public static class CitizenReportsAttachmentsFeatureInstaller
{
    public static IServiceCollection AddCitizenReportsAttachmentsFeature(this IServiceCollection services)
    {
        return services;
    }
}
