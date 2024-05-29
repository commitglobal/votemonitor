using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Vote.Monitor.Api.Feature.Auth.ResetPassword;

public class Endpoint(ILogger<Endpoint> logger, UserManager<ApplicationUser> userManager) : Endpoint<Request, Results<Ok, ProblemHttpResult>>
{
    public override void Configure()
    {
        Post("/api/auth/reset-password");
        AllowAnonymous();
    }

    public override async Task<Results<Ok, ProblemHttpResult>> ExecuteAsync(Request request, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(request.Email.Normalize());

        // Don't reveal that the user does not exist
        if (user is null)
        {
            logger.LogWarning("Possible user enumeration. Unknown email received {email}", request.Email);
            // Don't reveal that the user does not exist or is not confirmed
            return TypedResults.Ok();
        }

        var result = await userManager.ResetPasswordAsync(user, request.Token, request.Password);

        if (!result.Succeeded)
        {
            logger.LogWarning("Could not reset password for {email} {result}", request.Email, result);
        }

        return TypedResults.Ok();
    }
}
