using Microsoft.AspNetCore.Authentication.JwtBearer;
using Vote.Monitor.Api.Feature.Auth.Options;
using Vote.Monitor.Api.Feature.Auth.Services;

namespace Vote.Monitor.Api.Feature.Auth;

public static class AuthFeatureInstaller
{
    public const string SectionKey = "AuthFeatureConfig";
    public static IServiceCollection AddAuthFeature(this IServiceCollection services, IConfigurationSection config)
    {
        services.Configure<JWTConfig>(config.GetSection(JWTConfig.Key));

        services.AddSingleton<IConfigureOptions<JwtBearerOptions>, ConfigureJwtBearerOptions>();

        services
            .AddAuthentication(authentication =>
            {
                authentication.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authentication.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, null!);


        services.AddTransient<ITokenService, TokenService>();

        return services;
    }
}
