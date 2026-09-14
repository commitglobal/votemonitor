using Microsoft.Extensions.DependencyInjection;

namespace Feature.CitizenReports.Comments;

public static class CitizenReportsCommentsInstaller
{
    public static IServiceCollection AddCitizenReportsCommentsFeature(this IServiceCollection services)
    {
        return services;
    }
}
