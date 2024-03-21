using Authorization.Policies.Requirements;
using Authorization.Policies.Specifications;

namespace Authorization.Policies.RequirementsHandlers;

internal class NgoAdminAuthorizationHandler(ICurrentUserProvider currentUserProvider,
    IReadRepository<NgoAdmin> ngoAdminRepository) : AuthorizationHandler<NgoAdminRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, NgoAdminRequirement requirement)
    {
        if (!currentUserProvider.IsNgoAdmin())
        {
            context.Fail();
            return;
        }

        var ngoId = currentUserProvider.GetNgoId()!.Value;
        var ngoAdminId = currentUserProvider.GetUserId()!.Value;

        var result = await ngoAdminRepository.FirstOrDefaultAsync(new GetNgoAdminSpecification(ngoId, ngoAdminId));

        if (result is null)
        {
            context.Fail();
            return;
        }

        if (result.NgoStatus == NgoStatus.Deactivated)
        {
            context.Fail();
            return;
        }

        if (result.UserStatus == UserStatus.Deactivated)
        {
            context.Fail();
            return;
        }

        context.Succeed(requirement);
    }
}
