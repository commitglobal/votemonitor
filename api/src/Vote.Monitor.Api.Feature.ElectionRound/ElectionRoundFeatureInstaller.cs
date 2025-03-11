using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Vote.Monitor.Core.Converters;

namespace Vote.Monitor.Api.Feature.ElectionRound;

public static class ElectionRoundFeatureInstaller
{
    public static IServiceCollection AddElectionRoundFeature(this IServiceCollection services)
    {
        SqlMapper.AddTypeHandler(typeof(MonitoringNgoModel[]), new JsonToObjectConverter<MonitoringNgoModel[]>());

        return services;
    }
}
