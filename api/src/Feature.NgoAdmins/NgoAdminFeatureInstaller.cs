using Microsoft.Extensions.DependencyInjection;

namespace Feature.NgoAdmins;

public static class NgoAdminFeatureInstaller
{
    public static IServiceCollection AddNgoAdminFeature(this IServiceCollection services)
    {
        return services;
    }
}
