using Authorization.Policies;
using Microsoft.AspNetCore.Identity;

namespace Feature.Auth.SetUserPassword;

public class Endpoint(UserManager<ApplicationUser> userManager) : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Post("/api/auth/set-password");
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request request, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(request.AspNetUserId.ToString());
        if (user is null)
        {
            return TypedResults.NotFound();
        }
        user.PasswordHash = userManager.PasswordHasher.HashPassword(user, request.NewPassword);

        var res = await userManager.UpdateAsync(user);
        if (!res.Succeeded)
        {
            AddError(x => x.NewPassword, string.Join(";", res.Errors.Select(x => x.Description)));
            ThrowIfAnyErrors(500);
        }

        return TypedResults.NoContent();
    }
}
