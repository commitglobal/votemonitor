using Authorization.Policies.Requirements;
using Authorization.Policies.Specifications;
using Vote.Monitor.Domain.Entities.ElectionRoundAggregate;
using Vote.Monitor.Domain.Entities.NgoStaffAggregate;

namespace Authorization.Policies.RequirementHandlers;

internal class CitizenReportingNgoAdminOrStaffAuthorizationHandler(
    ICurrentUserProvider currentUserProvider,
    ICurrentUserRoleProvider currentUserRoleProvider,
    IReadRepository<ElectionRound> electionRoundRepository,
    IReadRepository<NgoStaff> ngoStaffRepository)
    : AuthorizationHandler<CitizenReportingNgoAdminOrStaffRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        CitizenReportingNgoAdminOrStaffRequirement requirement)
    {
        if (!currentUserRoleProvider.IsNgoAdmin() && !currentUserRoleProvider.IsNgoStaff())
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

        if (currentUserRoleProvider.IsNgoStaff())
        {
            var getNgoStaffSpecification =
                new GetNgoStaffSpecification(ngoId.Value, currentUserProvider.GetUserId()!.Value);
            var staff = await ngoStaffRepository.FirstOrDefaultAsync(getNgoStaffSpecification);

            if (staff is null || staff.UserStatus == UserStatus.Deactivated)
            {
                context.Fail();
                return;
            }
        }

        context.Succeed(requirement);
    }
}
