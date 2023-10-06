
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;



namespace Vote.Monitor.Core;
public static  class CoreSetupStartup
{
    public static IServiceCollection AddCoreStartup(this IServiceCollection services, IConfiguration config)
    {

      
        return services;
    }
}
