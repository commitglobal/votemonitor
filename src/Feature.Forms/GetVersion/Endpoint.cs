using Authorization.Policies.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Core.Services.Security;
using Vote.Monitor.Domain;

namespace Feature.Forms.GetVersion;

public class Endpoint(
    IAuthorizationService authorizationService,
    ICurrentUserRoleProvider currentUserRoleProvider,
    VoteMonitorContext context) : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/forms/:version");
        DontAutoTag();
        Options(x => x.WithTags("forms", "mobile"));
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
        //var requirement = new MonitoringObserverRequirement(req.ElectionRoundId);
        //var authorizationResult = await authorizationService.AuthorizeAsync(User, requirement);
        //if (!authorizationResult.Succeeded)
        //{
        //    return TypedResults.NotFound();
        //}

        //var monitoringNgoId = await currentUserRoleProvider.GetMonitoringNgoId(req.ElectionRoundId);
        //if (monitoringNgoId == null)
        //{
        //    return TypedResults.NotFound();
        //}

        //var monitoringNgo = await context.MonitoringNgos
        //    .Where(x => x.Id == monitoringNgoId)
        //    .Select(x => new { x.FormStationsVersion, x.Id })
        //    .FirstOrDefaultAsync(ct);

        //if (monitoringNgo is null)
        //{
        //    return TypedResults.NotFound();
        //}

        //return TypedResults.Ok(new Response()
        //{
        //    ElectionRoundId = req.ElectionRoundId,
        //    Version = monitoringNgo.FormStationsVersion
        //});
    }
}
