using Authorization.Policies.Requirements;
using Authorization.Policies.Specifications;
using Vote.Monitor.Domain.Entities.ElectionRoundAggregate;

namespace Authorization.Policies.RequirementHandlers;

internal class MonitoringNgoAdminOrObserverAuthorizationHandler(
    ICurrentUserProvider currentUserProvider,
    ICurrentUserRoleProvider currentUserRoleProvider,
    IReadRepository<MonitoringObserver> monitoringObserverRepository,
    IReadRepository<MonitoringNgo> monitoringNgoRepository)
    : AuthorizationHandler<MonitoringNgoAdminOrObserverRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        MonitoringNgoAdminOrObserverRequirement requirement)
    {
        if (!currentUserRoleProvider.IsObserver() && !currentUserRoleProvider.IsNgoAdmin())
        {
            context.Fail();
            return;
        }

        if (currentUserRoleProvider.IsObserver())
        {
            var observerId = currentUserProvider.GetUserId()!.Value;
            var specification = new GetMonitoringObserverSpecification(requirement.ElectionRoundId, observerId);
            var result = await monitoringObserverRepository.FirstOrDefaultAsync(specification);

            if (result is not null)
            {
                if (result.ElectionRoundStatus == ElectionRoundStatus.Archived ||
                    result.NgoStatus == NgoStatus.Deactivated ||
                    result.MonitoringNgoStatus == MonitoringNgoStatus.Suspended ||
                    result.UserStatus == UserStatus.Deactivated ||
                    result.MonitoringObserverStatus == MonitoringObserverStatus.Suspended)
                {
                    context.Fail();
                    return;
                }

                context.Succeed(requirement);
                return;
            }
        }

        if (currentUserRoleProvider.IsNgoAdmin())
        {
            var ngoId = currentUserProvider.GetNgoId();
            var getMonitoringNgoSpecification =
                new GetMonitoringNgoSpecification(requirement.ElectionRoundId, ngoId!.Value);
            var ngoAdminResult = await monitoringNgoRepository.FirstOrDefaultAsync(getMonitoringNgoSpecification);

            if (ngoAdminResult is not null)
            {
                if (ngoAdminResult.ElectionRoundStatus == ElectionRoundStatus.Archived ||
                    ngoAdminResult.NgoStatus == NgoStatus.Deactivated ||
                    ngoAdminResult.MonitoringNgoStatus == MonitoringNgoStatus.Suspended)
                {
                    context.Fail();
                    return;
                }

                context.Succeed(requirement);
                return;
            }
        }

        context.Fail();
    }
}