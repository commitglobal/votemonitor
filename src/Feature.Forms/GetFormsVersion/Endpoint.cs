using Authorization.Policies.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Domain;

namespace Feature.Forms.GetFormsVersion;

public class Endpoint(IAuthorizationService authorizationService, VoteMonitorContext context) : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/forms:version");
        DontAutoTag();
        Options(x => x.WithTags("forms", "mobile"));
        Summary(s =>
        {
            s.Summary = "Gets current version of forms for users monitoring ngo";
            s.Description = "Cache key changes every time any form changes";
        });
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var requirement = new MonitoringObserverRequirement(req.ElectionRoundId);
        var authorizationResult = await authorizationService.AuthorizeAsync(User, requirement);
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var monitoringNgo = await context.MonitoringNgos
            .Where(x => x.ElectionRoundId == req.ElectionRoundId)
            .Select(x => new { x.FormsVersion, x.Id })
            .FirstOrDefaultAsync(ct);

        if (monitoringNgo is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(new Response
        {
            ElectionRoundId = monitoringNgo.Id,
            CacheKey = monitoringNgo.FormsVersion.ToString()
        });
    }
}
