using Authorization.Policies.Requirements;
using Authorization.Policies.Specifications;

namespace Authorization.Policies.RequirementHandlers;

internal class MonitoringObserverAuthorizationHandler(
    ICurrentUserProvider currentUserProvider,
    IReadRepository<MonitoringObserver> monitoringObserverRepository)
    : AuthorizationHandler<MonitoringObserverRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        MonitoringObserverRequirement requirement)
    {
        if (!currentUserProvider.IsObserver())
        {
            return;
        }

        var observerId = currentUserProvider.GetUserId()!.Value;
        var specification = new GetMonitoringObserverSpecification(requirement.ElectionRoundId, observerId);
        var result = await monitoringObserverRepository.FirstOrDefaultAsync(specification);

        if (result is null)
        {
            context.Fail();
            return;
        }

        if (result.NgoStatus == NgoStatus.Deactivated || result.MonitoringNgoStatus == MonitoringNgoStatus.Suspended)
        {
            context.Fail();
            return;
        }

        if (result.UserStatus == UserStatus.Deactivated || result.MonitoringObserverStatus == MonitoringObserverStatus.Suspended)
        {
            context.Fail();
            return;
        }

        context.Succeed(requirement);
    }
}
