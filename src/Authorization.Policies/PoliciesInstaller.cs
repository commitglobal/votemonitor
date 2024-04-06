using Authorization.Policies.RequirementHandlers;
using Microsoft.Extensions.DependencyInjection;
using Vote.Monitor.Core.Security;

namespace Authorization.Policies;

public static class AuthorizationPoliciesInstaller
{
    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddScoped<IAuthorizationHandler, MonitoringNgoAdminAuthorizationHandler>();
        services.AddScoped<IAuthorizationHandler, MonitoringNgoAdminOrObserverAuthorizationHandler>();
        services.AddScoped<IAuthorizationHandler, MonitoringObserverAuthorizationHandler>();
        services.AddScoped<IAuthorizationHandler, NgoAdminAuthorizationHandler>();

        services.AddAuthorization(options =>
        {
            options.AddPolicy(PolicyNames.PlatformAdminsOnly, policy => policy.RequireRole(UserRole.PlatformAdmin));
            options.AddPolicy(PolicyNames.NgoAdminsOnly, policy => policy.RequireRole(UserRole.NgoAdmin));
            options.AddPolicy(PolicyNames.ObserversOnly, policy => policy.RequireRole(UserRole.Observer));
            options.AddPolicy(PolicyNames.AdminsOnly, policy => policy.RequireRole(UserRole.PlatformAdmin, UserRole.NgoAdmin));
        });

        services
            .AddScoped<ICurrentUserIdProvider, CurrentUserIdProvider>()
            .AddScoped<ICurrentUserRoleProvider, CurrentUserRoleProvider>()
            .AddScoped(sp => (ICurrentUserInitializer)sp.GetRequiredService<ICurrentUserIdProvider>());

        return services;
    }
}
