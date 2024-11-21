using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Domain;

namespace Feature.Form.Submissions.Delete;

public class Endpoint(IAuthorizationService authorizationService, VoteMonitorContext context)
    : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/form-submissions");
        DontAutoTag();
        Options(x => x.WithTags("form-submissions", "mobile"));
        Summary(s => { s.Summary = "Deletes a form submission for a polling station"; });

        Policies(PolicyNames.ObserversOnly);
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringObserverRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
           return TypedResults.NotFound();
        }

        await context.FormSubmissions
            .Where(x => x.ElectionRoundId == req.ElectionRoundId
                        && x.FormId == req.FormId
                        && x.Form.ElectionRoundId == req.ElectionRoundId
                        && x.MonitoringObserver.ObserverId == req.ObserverId
                        && x.MonitoringObserver.ElectionRoundId == req.ElectionRoundId
                        && x.PollingStationId == req.PollingStationId)
            .ExecuteDeleteAsync(ct);

        await context.Notes
            .Where(x => x.ElectionRoundId == req.ElectionRoundId
                        && x.FormId == req.FormId
                        && x.Form.ElectionRoundId == req.ElectionRoundId
                        && x.MonitoringObserver.ObserverId == req.ObserverId
                        && x.MonitoringObserver.ElectionRoundId == req.ElectionRoundId
                        && x.PollingStationId == req.PollingStationId)
            .ExecuteDeleteAsync(ct);

        await context.Attachments
            .Where(x => x.ElectionRoundId == req.ElectionRoundId
                        && x.FormId == req.FormId
                        && x.Form.ElectionRoundId == req.ElectionRoundId
                        && x.MonitoringObserver.ObserverId == req.ObserverId
                        && x.MonitoringObserver.ElectionRoundId == req.ElectionRoundId
                        && x.PollingStationId == req.PollingStationId)
            .ExecuteUpdateAsync(x => x.SetProperty(a => a.IsDeleted, true), ct);

        return TypedResults.NoContent();
    }
}
