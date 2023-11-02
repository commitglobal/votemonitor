using FastEndpoints.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vote.Monitor.Auth.Options;

namespace Vote.Monitor.Auth;

public static class AuthFeatureInstaller
{
    public const string SectionKey = "AuthFeatureConfig";
    public static IServiceCollection AddAuthFeature(this IServiceCollection services, IConfigurationSection config)
    {
        services.Configure<JWTConfig>(config.GetSection(JWTConfig.Key));

        services.AddJWTBearerAuth(config[$"{JWTConfig.Key}:TokenSigningKey"]!);
        services.AddAuthorization();

        return services;
    }
}
