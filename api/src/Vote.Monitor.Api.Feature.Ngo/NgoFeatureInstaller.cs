using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Vote.Monitor.Core.Converters;

namespace Vote.Monitor.Api.Feature.Ngo;

public static class NgoFeatureInstaller
{
    public static IServiceCollection AddNgoFeature(this IServiceCollection services)
    {
        SqlMapper.AddTypeHandler(typeof(NgoModel.MonitoredElectionsModel[]),
            new JsonToObjectConverter<NgoModel.MonitoredElectionsModel[]>());

        return services;
    }
}
