using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vote.Monitor.Feature.PollingStation.Repositories;

namespace Vote.Monitor.Feature.PollingStation;
public static class PollingStationFeatureInstaller
{
    public const string SectionKey = "PollingStations";
    public static IServiceCollection AddPollingStationFeatures(this IServiceCollection services, IConfigurationSection config)
    {
        
        services.AddScoped<IPollingStationRepository , PollingStationRepository>();
        return services;
    }
}
