using Authorization.Policies.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace Vote.Monitor.Api.Feature.Auth.MonitoringNgoAdminOrObserver;

public class Endpoint(IAuthorizationService authorizationService) : Endpoint<Request, Results<Ok<string>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/auth/monitoringNgoAdminsOrObserversGreeting");
        DontAutoTag();
        Options(x => x.WithTags("test-auth-policies"));
    }

    public override async Task<Results<Ok<string>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult = await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminOrObserverRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok("Hello monitoring ngo admin or observer!");
    }
}
