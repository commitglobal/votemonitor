using Microsoft.AspNetCore.Identity;
using Vote.Monitor.Core.Extensions;

namespace Vote.Monitor.Api.Feature.Auth.ChangePassword;

public class Endpoint(UserManager<ApplicationUser> userManager) : Endpoint<Request, Results<Ok, ProblemHttpResult>>
{
    public override void Configure()
    {
        Post("/api/auth/change-password");
    }

    public override async Task<Results<Ok, ProblemHttpResult>> ExecuteAsync(Request request, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(request.UserId);

        if (user is null)
        {
            return TypedResults.Problem();
        }

        var result = await userManager.ChangePasswordAsync(user, request.Password, request.NewPassword);

        return result.Succeeded
            ? TypedResults.Ok()
            : TypedResults.Problem(string.Join(",", result.GetErrors()));
    }
}
