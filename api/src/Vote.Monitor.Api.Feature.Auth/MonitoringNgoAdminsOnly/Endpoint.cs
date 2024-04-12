using Authorization.Policies.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace Vote.Monitor.Api.Feature.Auth.MonitoringNgoAdminsOnly;

public class Endpoint(IAuthorizationService authorizationService) : Endpoint<Request, Results<Ok<string>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/auth/monitoringNgoAdminsGreeting");
        DontAutoTag();
        Options(x => x.WithTags("test-auth-policies"));
    }

    public override async Task<Results<Ok<string>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult = await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok("Hello monitoring ngo admin!");
    }
}
