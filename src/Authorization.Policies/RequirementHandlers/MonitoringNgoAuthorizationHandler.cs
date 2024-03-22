using Authorization.Policies.Requirements;
using Authorization.Policies.Specifications;

namespace Authorization.Policies.RequirementHandlers;

internal class MonitoringNgoAuthorizationHandler(
    ICurrentUserProvider currentUserProvider,
    IReadRepository<MonitoringNgo> monitoringNgoRepository)
    : AuthorizationHandler<MonitoringNgoRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, MonitoringNgoRequirement requirement)
    {
        if (!currentUserProvider.IsNgoAdmin())
        {
            context.Fail();
            return;
        }

        var ngoId = currentUserProvider.GetNgoId()!.Value;
        var getMonitoringNgoSpecification = new GetMonitoringNgoSpecification(requirement.ElectionRoundId, ngoId);
        var result = await monitoringNgoRepository.FirstOrDefaultAsync(getMonitoringNgoSpecification);

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

        context.Succeed(requirement);
    }
}
