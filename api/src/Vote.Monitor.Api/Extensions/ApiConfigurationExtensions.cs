using Vote.Monitor.Core.Options;

namespace Vote.Monitor.Api.Extensions;

public static class ApiConfigurationExtensions
{
    public static IServiceCollection AddApiConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ApiConfiguration>(configuration.GetSection(ApiConfiguration.Key));

        return services;
    }
}
