using Authorization.Policies.Requirements;
using Authorization.Policies.Specifications;
using Vote.Monitor.Domain.Entities.CoalitionAggregate;
using Vote.Monitor.Domain.Entities.ElectionRoundAggregate;

namespace Authorization.Policies.RequirementHandlers;

internal class CoalitionLeaderAuthorizationHandler(
    ICurrentUserProvider currentUserProvider,
    ICurrentUserRoleProvider currentUserRoleProvider,
    IReadRepository<Coalition> coalitionRepository)
    : AuthorizationHandler<CoalitionLeaderRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        CoalitionLeaderRequirement requirement)
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

        var coalitionLeaderSpecification = new GetCoalitionLeaderDetailsSpecification(requirement.ElectionRoundId, requirement.CoalitionId, ngoId.Value);
        var result = await coalitionRepository.FirstOrDefaultAsync(coalitionLeaderSpecification);

        if (result is null)
        {
            context.Fail();
            return;
        }

        if (result.ElectionRoundStatus == ElectionRoundStatus.Archived
            || result.NgoStatus == NgoStatus.Deactivated
            || result.MonitoringNgoStatus == MonitoringNgoStatus.Suspended)
        {
            context.Fail();
            return;
        }

        context.Succeed(requirement);
    }
}
