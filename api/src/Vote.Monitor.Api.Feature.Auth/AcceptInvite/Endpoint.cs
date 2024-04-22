using Microsoft.AspNetCore.Identity;
using Vote.Monitor.Api.Feature.Auth.Specifications;
using Vote.Monitor.Core.Extensions;

namespace Vote.Monitor.Api.Feature.Auth.AcceptInvite;

public class Endpoint(UserManager<ApplicationUser> userManager, IReadRepository<ApplicationUser> repository) : Endpoint<Request, Results<Ok, ProblemHttpResult>>
{
    public override void Configure()
    {
        Post("/api/auth/accept-invite");
    }

    public override async Task<Results<Ok, ProblemHttpResult>> ExecuteAsync(Request request, CancellationToken ct)
    {
        var user = await repository.FirstOrDefaultAsync(new GetByInvitationCode(request.InvitationToken), ct);

        if (user is null)
        {
            return TypedResults.Problem();
        }
        user.AcceptInvite(request.Password);
        var result = await userManager.ChangePasswordAsync(user, request.Password, request.NewPassword);

        return result.Succeeded
            ? TypedResults.Ok()
            : TypedResults.Problem(string.Join(",", result.GetErrors()));
    }
}
