using Authorization.Policies.RequirementsHandlers;
using Microsoft.Extensions.DependencyInjection;

namespace Authorization.Policies;

public static class AuthorizationPoliciesInstaller
{
    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddScoped<IAuthorizationHandler, MonitoringNgoAuthorizationHandler>();
        services.AddScoped<IAuthorizationHandler, MonitoringObserverAuthorizationHandler>();

        services.AddAuthorization(options =>
        {
            options.AddPolicy(PolicyNames.PlatformAdminsOnly, policy => policy.RequireRole(UserRole.PlatformAdmin));
            options.AddPolicy(PolicyNames.NgoAdminsOnly, policy => policy.RequireRole(UserRole.NgoAdmin));
            options.AddPolicy(PolicyNames.ObserversOnly, policy => policy.RequireRole(UserRole.Observer));
        });

        return services;
    }
}
