using Microsoft.Extensions.DependencyInjection;

namespace Feature.Languages;

public static class LanguageFeatureInstaller
{
    public static IServiceCollection AddLanguageFeature(this IServiceCollection services)
    {
        return services;
    }
}
