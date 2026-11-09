using Feature.Labels.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Feature.Labels;

public static class LabelsFeatureInstaller
{
    public static IServiceCollection AddLabelsFeature(this IServiceCollection services)
    {
        services.AddScoped<ILabelService, LabelService>();
        return services;
    }
}
