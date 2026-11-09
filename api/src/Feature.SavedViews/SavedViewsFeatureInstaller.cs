using Microsoft.Extensions.DependencyInjection;

namespace Feature.SavedViews;

public static class SavedViewsFeatureInstaller
{
    public static IServiceCollection AddSavedViewsFeature(this IServiceCollection services)
    {
        return services;
    }
}
