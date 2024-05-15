using Microsoft.Extensions.DependencyInjection;

namespace Feature.ImportErrors;

public static class ImportErrorsFeatureInstaller
{
    public static IServiceCollection AddImportErrorsFeature(this IServiceCollection services)
    {
        return services;
    }
}
