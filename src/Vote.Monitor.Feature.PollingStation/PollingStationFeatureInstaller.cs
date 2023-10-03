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
    public const string SectionKey = "ExampleFeatures";
    public static IServiceCollection AddPollingStationFeatures(this IServiceCollection services, IConfigurationSection config)
    {
        
        services.AddSingleton<IPollingStationRepository , PollingStationRepository>();
        return services;
    }
}
