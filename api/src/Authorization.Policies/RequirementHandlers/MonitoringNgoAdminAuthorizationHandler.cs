﻿using Authorization.Policies.Requirements;
using Authorization.Policies.Specifications;

namespace Authorization.Policies.RequirementHandlers;

internal class MonitoringNgoAdminAuthorizationHandler(
    ICurrentUserProvider currentUserProvider,
    ICurrentUserRoleProvider currentUserRoleProvider,
    IReadRepository<MonitoringNgo> monitoringNgoRepository)
    : AuthorizationHandler<MonitoringNgoAdminRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, MonitoringNgoAdminRequirement adminRequirement)
    {
        if (!currentUserRoleProvider.IsNgoAdmin())
        {
            context.Fail();
            return;
        }

        var ngoId = currentUserProvider.GetNgoId();
        if (ngoId is null)
        {
            context.Fail();
            return;
        }
        
        
        var getMonitoringNgoSpecification = new GetMonitoringNgoSpecification(adminRequirement.ElectionRoundId, ngoId.Value);
        var result = await monitoringNgoRepository.FirstOrDefaultAsync(getMonitoringNgoSpecification);

        if (result is null)
        {
            context.Fail();
            return;
        }

        if (result.NgoStatus == NgoStatus.Deactivated
            || result.MonitoringNgoStatus == MonitoringNgoStatus.Suspended)
        {
            context.Fail();
            return;
        }

        context.Succeed(adminRequirement);
    }
}