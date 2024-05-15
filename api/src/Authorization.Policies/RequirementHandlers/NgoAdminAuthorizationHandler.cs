using Authorization.Policies.Requirements;
using Authorization.Policies.Specifications;
using Vote.Monitor.Domain.Entities.NgoAdminAggregate;

namespace Authorization.Policies.RequirementHandlers;

internal class NgoAdminAuthorizationHandler(ICurrentUserIdProvider currentUserIdProvider,
    ICurrentUserRoleProvider currentUserRoleProvider,
    IReadRepository<NgoAdmin> ngoAdminRepository) : AuthorizationHandler<NgoAdminRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, NgoAdminRequirement requirement)
    {
        if (!currentUserRoleProvider.IsNgoAdmin())
        {
            context.Fail();
            return;
        }

        var ngoId = await currentUserRoleProvider.GetNgoId();
        var ngoAdminId = currentUserIdProvider.GetUserId()!.Value;

        var result = await ngoAdminRepository.FirstOrDefaultAsync(new GetNgoAdminSpecification(ngoId!.Value, ngoAdminId));

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
