using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vote.Monitor.Feature.PollingStation.Data;
using Vote.Monitor.Feature.PollingStation.Repositories;

namespace Vote.Monitor.Feature.PollingStation;
public static class PollingStationFeatureInstaller
{
    public const string SectionKey = "PollingStations";
    public static IServiceCollection AddPollingStationFeatures(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<PollingStationDbContext>(options =>
        {

            options.UseNpgsql(config.GetConnectionString("PostgresVoteApiConnctionString"));
        });
        services.AddScoped<IPollingStationRepository , PollingStationRepository>();
        return services;
    }
}
