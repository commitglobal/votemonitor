using Authorization.Policies.Specifications;
using Vote.Monitor.Core.Security;

namespace Authorization.Policies;

public class CurrentUserRoleProvider : ICurrentUserRoleProvider
{
    private readonly ICurrentUserIdProvider _currentUserIdProvider;
    private readonly IReadRepository<MonitoringObserver> _monitoringObserverRepository;
    
    public CurrentUserRoleProvider(IReadRepository<MonitoringObserver> monitoringObserverRepository, 
        ICurrentUserIdProvider currentUserIdProvider)
    {
        _monitoringObserverRepository = monitoringObserverRepository;
        _currentUserIdProvider = currentUserIdProvider;
    }

    public async Task<Guid?> GetNgoId()
    {
        if (!IsAuthenticated())
        {
            return null;
        }

        var ngoIdClaimValue = _currentUserIdProvider.GetClaimValue(ApplicationClaimTypes.NgoId);

        if (!string.IsNullOrWhiteSpace(ngoIdClaimValue))
        {
            return Guid.Parse(ngoIdClaimValue);
        }

        var specification = new GetMonitoringObserverSpecification(_currentUserIdProvider.GetUserId());
        var monitoringObserver = await _monitoringObserverRepository.FirstOrDefaultAsync(specification);
        return monitoringObserver?.NgoId;
    }

    public bool IsAuthenticated() => _currentUserIdProvider.User?.Identity?.IsAuthenticated is true;

    public bool IsPlatformAdmin() => _currentUserIdProvider.User?.IsInRole(UserRole.PlatformAdmin.Value) is true;

    public bool IsNgoAdmin() => _currentUserIdProvider.User?.IsInRole(UserRole.NgoAdmin.Value) is true;

    public bool IsObserver() => _currentUserIdProvider.User?.IsInRole(UserRole.Observer.Value) is true;
}
