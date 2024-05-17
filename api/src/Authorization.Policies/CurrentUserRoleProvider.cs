using Vote.Monitor.Core.Security;

namespace Authorization.Policies;

public class CurrentUserRoleProvider(ICurrentUserProvider currentUserProvider) : ICurrentUserRoleProvider
{
    public bool IsAuthenticated() => currentUserProvider.User?.Identity?.IsAuthenticated is true;

    public bool IsPlatformAdmin() => currentUserProvider.User?.IsInRole(UserRole.PlatformAdmin.Value) is true;

    public bool IsNgoAdmin() => currentUserProvider.User?.IsInRole(UserRole.NgoAdmin.Value) is true;

    public bool IsObserver() => currentUserProvider.User?.IsInRole(UserRole.Observer.Value) is true;
}
