using Authorization.Policies.Requirements;
using Authorization.Policies.Specifications;

namespace Authorization.Policies.RequirementHandlers;

internal class IsResourceOwnerRequirementHandler(ICurrentUserProvider currentUserProvider, ICurrentUserRoleProvider currentUserRoleProvider,
    IReadRepository<MonitoringObserver> monitoringObserverRepository) : AuthorizationHandler<IsResourceOwnerRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, IsResourceOwnerRequirement requirement)
    {
        // When a ngo admin tries to access a resource check if they are monitoring ngo, and that resource was created by a monitoring observer
        if (currentUserRoleProvider.IsNgoAdmin())
        {
            var ngoId = currentUserProvider.GetNgoId();

            var specification = new GetMonitoringObserverSpecification(requirement.ElectionRoundId,
                ngoId,
                requirement.Resource.CreatedBy);
            var result = await monitoringObserverRepository
                .FirstOrDefaultAsync(specification);

            if (result != null)
            {
                context.Succeed(requirement);
                return;
            }
        }

        if (currentUserRoleProvider.IsObserver())
        {
            if (currentUserProvider.GetUserId() == requirement.Resource.CreatedBy)
            {
                context.Succeed(requirement);
            }
        }

        context.Fail();
    }
}
