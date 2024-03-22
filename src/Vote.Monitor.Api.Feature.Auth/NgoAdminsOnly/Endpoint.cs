using Authorization.Policies;
using Authorization.Policies.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace Vote.Monitor.Api.Feature.Auth.NgoAdminsOnly;

public class Endpoint(IAuthorizationService authorizationService) : Endpoint<Request, Results<Ok<string>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/auth/ngoAdminsGreeting");
        DontAutoTag();
        Options(x => x.WithTags("test-auth-policies"));
    }

    public override async Task<Results<Ok<string>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult = await authorizationService.AuthorizeAsync(User, new NgoAdminRequirement(req.NgoId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok("Hello ngo admin!");
    }
}
