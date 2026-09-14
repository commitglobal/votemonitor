using Microsoft.Extensions.DependencyInjection;

namespace Feature.QuickReports.Comments;

public static class QuickReportsCommentsInstaller
{
    public static IServiceCollection AddQuickReportsCommentsFeature(this IServiceCollection services)
    {
        return services;
    }
}
