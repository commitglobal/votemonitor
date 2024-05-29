using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Vote.Monitor.Core.Extensions;

namespace Vote.Monitor.Api.Feature.Auth.ChangePassword;

public class Endpoint(ILogger<Endpoint> logger, UserManager<ApplicationUser> userManager) : Endpoint<Request, Results<Ok, ProblemHttpResult>>
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
            logger.LogWarning("Could not find user by id {userId}", request.UserId);
            // Don't reveal that the user does not exist or is not confirmed
            return TypedResults.Ok();
        }

        var result = await userManager.ChangePasswordAsync(user, request.Password, request.NewPassword);

        return result.Succeeded
            ? TypedResults.Ok()
            : TypedResults.Problem(string.Join(",", result.GetErrors()));
    }
}
