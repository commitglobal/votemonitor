using Microsoft.AspNetCore.Identity;

namespace Vote.Monitor.Api.Feature.Auth.ResetPassword;

public class Endpoint(UserManager<ApplicationUser> userManager) : Endpoint<Request, Results<Ok, ProblemHttpResult>>
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
            return TypedResults.Problem();
        }

        var result = await userManager.ResetPasswordAsync(user, request.Token, request.Password);

        return result.Succeeded
            ? TypedResults.Ok()
            : TypedResults.Problem();
    }
}
