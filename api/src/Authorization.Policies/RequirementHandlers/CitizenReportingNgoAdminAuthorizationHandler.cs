using Authorization.Policies.Requirements;
using Authorization.Policies.Specifications;
using Vote.Monitor.Domain.Entities.ElectionRoundAggregate;

namespace Authorization.Policies.RequirementHandlers;

internal class CitizenReportingNgoAdminAuthorizationHandler(
    ICurrentUserProvider currentUserProvider,
    ICurrentUserRoleProvider currentUserRoleProvider,
    IReadRepository<ElectionRound> electionRoundRepository)
    : AuthorizationHandler<CitizenReportingNgoAdminRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        CitizenReportingNgoAdminRequirement requirement)
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

        var getMonitoringNgoSpecification =
            new GetCitizenReportingMonitoringNgoSpecification(requirement.ElectionRoundId, ngoId.Value);
        var result = await electionRoundRepository.FirstOrDefaultAsync(getMonitoringNgoSpecification);

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

        context.Succeed(requirement);
    }
}
