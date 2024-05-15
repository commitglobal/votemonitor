﻿using Authorization.Policies.Requirements;
using Authorization.Policies.Specifications;
using Vote.Monitor.Domain.Entities.ElectionRoundAggregate;

namespace Authorization.Policies.RequirementHandlers;

internal class MonitoringObserverAuthorizationHandler(
    ICurrentUserIdProvider currentUserIdProvider,
    ICurrentUserRoleProvider currentUserRoleProvider,
    IReadRepository<MonitoringObserver> monitoringObserverRepository)
    : AuthorizationHandler<MonitoringObserverRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        MonitoringObserverRequirement requirement)
    {
        if (!currentUserRoleProvider.IsObserver())
        {
            return;
        }

        var observerId = currentUserIdProvider.GetUserId()!.Value;
        var specification = new GetMonitoringObserverSpecification(requirement.ElectionRoundId, observerId);
        var result = await monitoringObserverRepository.FirstOrDefaultAsync(specification);

        if (result is null)
        {
            context.Fail();
            return;
        }

        if (result.ElectionRoundStatus == ElectionRoundStatus.Archived)
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
