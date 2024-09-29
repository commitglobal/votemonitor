using Authorization.Policies.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Domain;

namespace Feature.IssueReports.UpdateStatus;

public class Endpoint(IAuthorizationService authorizationService,VoteMonitorContext context) : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Put("/api/election-rounds/{electionRoundId}/issue-reports/{id}:status");
        DontAutoTag();
        Options(x => x.WithTags("issue-reports"));
        Summary(s => { s.Summary = "Updates follow up status for a issue report"; });

        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }
        
        await context.IssueReports
            .Where(x => x.Form.MonitoringNgo.NgoId == req.NgoId
                        && x.ElectionRoundId == req.ElectionRoundId
                        && x.Id == req.Id)
            .ExecuteUpdateAsync(x => x.SetProperty(p => p.FollowUpStatus, req.FollowUpStatus), cancellationToken: ct);

        return TypedResults.NoContent();
    }
}