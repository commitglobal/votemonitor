using Dapper;
using Feature.ObserverGuide.Model;
using Microsoft.Extensions.DependencyInjection;
using Vote.Monitor.Core.Converters;

namespace Feature.ObserverGuide;

public static class ObserverGuideFeatureInstaller
{
    public static IServiceCollection AddObserverGuideFeature(this IServiceCollection services)
    {
        SqlMapper.AddTypeHandler(typeof(GuideAccessModel[]), new JsonToObjectConverter<GuideAccessModel[]>());

        return services;
    }
}
