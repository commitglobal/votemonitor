using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Domain;

namespace Feature.Form.Submissions.UpdateStatus;

public class Endpoint(IAuthorizationService authorizationService, VoteMonitorContext context)
    : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Put("/api/election-rounds/{electionRoundId}/form-submissions/{id}:status");
        DontAutoTag();
        Options(x => x.WithTags("form-submissions"));
        Summary(s => { s.Summary = "Updates follow up status for a submission"; });

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
        
        await context.PollingStationInformation
            .Where(x => x.MonitoringObserver.MonitoringNgo.NgoId == req.NgoId
                        && x.ElectionRoundId == req.ElectionRoundId
                        && x.Id == req.Id)
            .ExecuteUpdateAsync(x => x.SetProperty(p => p.FollowUpStatus, req.FollowUpStatus), cancellationToken: ct);

        await context.FormSubmissions
            .Where(x => x.MonitoringObserver.MonitoringNgo.NgoId == req.NgoId
                        && x.ElectionRoundId == req.ElectionRoundId
                        && x.Id == req.Id)
            .ExecuteUpdateAsync(x => x.SetProperty(p => p.FollowUpStatus, req.FollowUpStatus), cancellationToken: ct);

        return TypedResults.NoContent();
    }
}