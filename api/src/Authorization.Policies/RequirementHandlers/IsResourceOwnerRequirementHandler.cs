using Authorization.Policies.Requirements;

namespace Authorization.Policies.RequirementHandlers;

internal class IsResourceOwnerRequirementHandler(ICurrentUserIdProvider currentUserIdProvider) : AuthorizationHandler<IsResourceOwnerRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, IsResourceOwnerRequirement requirement)
    {
        if (currentUserIdProvider.GetUserId() == null)
        {
            context.Fail();
            return;
        }

        if (currentUserIdProvider.GetUserId().Value != requirement.Resource.CreatedBy)
        {
            context.Fail();
            return;
        }

        context.Succeed(requirement);
    }
}
