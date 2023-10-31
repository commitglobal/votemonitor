using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vote.Monitor.Feature.PollingStation.Repositories;

namespace Vote.Monitor.Feature.PollingStation;
public static class PollingStationFeatureInstaller
{
    public static IServiceCollection AddPollingStationFeature(this IServiceCollection services)
    {
        
        services.AddScoped<IPollingStationRepository , PollingStationRepository>();
        return services;
    }
}
