using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vote.Monitor.Domain.DataContext;

namespace Vote.Monitor.Domain;

public static  class DomainSetup
{
    public static IServiceCollection DomainSetupStartup(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<PollingStationDbContext>(options =>
        {

            options.UseNpgsql(config.GetConnectionString("PostgresVoteApiConnctionString"));
        });
       
        return services;
    }
}
