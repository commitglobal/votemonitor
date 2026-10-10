using Authorization.Policies.Requirements;
using Authorization.Policies.Specifications;
using Vote.Monitor.Domain.Entities.NgoAdminAggregate;
using Vote.Monitor.Domain.Entities.NgoStaffAggregate;

namespace Authorization.Policies.RequirementHandlers;

internal class MonitoringNgoAdminOrStaffAuthorizationHandler(
    ICurrentUserProvider currentUserProvider,
    ICurrentUserRoleProvider currentUserRoleProvider,
    IReadRepository<MonitoringNgo> monitoringNgoRepository,
    IReadRepository<NgoAdmin> ngoAdminRepository,
    IReadRepository<NgoStaff> ngoStaffRepository)
    : AuthorizationHandler<MonitoringNgoAdminOrStaffRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        MonitoringNgoAdminOrStaffRequirement requirement)
    {
        if (!currentUserRoleProvider.IsNgoAdmin() || currentUserRoleProvider.IsNgoStaff())
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

        var userId = currentUserProvider.GetUserId();
        if (userId is null)
        {
            context.Fail();
            return;
        }

        var getMonitoringNgoSpecification =
            new GetMonitoringNgoSpecification(requirement.ElectionRoundId, ngoId.Value);
        var monitoringNgo = await monitoringNgoRepository.FirstOrDefaultAsync(getMonitoringNgoSpecification);

        if (monitoringNgo is null)
        {
            context.Fail();
            return;
        }

        if (monitoringNgo.NgoStatus == NgoStatus.Deactivated
            || monitoringNgo.MonitoringNgoStatus == MonitoringNgoStatus.Suspended)
        {
            context.Fail();
            return;
        }

        if (currentUserRoleProvider.IsNgoAdmin())
        {
            var getNgoAdminSpecification =
                new GetNgoAdminSpecification(ngoId.Value, currentUserProvider.GetUserId()!.Value);
            var admin = await ngoAdminRepository.FirstOrDefaultAsync(getNgoAdminSpecification);

            if (admin is null)
            {
                context.Fail();
                return;
            }

            if (admin.UserStatus == UserStatus.Deactivated)
            {
                context.Fail();
                return;
            }
        }

        if (currentUserRoleProvider.IsNgoStaff())
        {
            var getNgoStaffSpecification =
                new GetNgoStaffSpecification(ngoId.Value, currentUserProvider.GetUserId()!.Value);
            var staff = await ngoStaffRepository.FirstOrDefaultAsync(getNgoStaffSpecification);

            if (staff is null)
            {
                context.Fail();
                return;
            }

            if (staff.UserStatus == UserStatus.Deactivated)
            {
                context.Fail();
                return;
            }
        }


        context.Succeed(requirement);
    }
}
