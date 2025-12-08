using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Domain;

namespace Feature.Form.Submissions.SetCompletionV2;

public class Endpoint(IAuthorizationService authorizationService, VoteMonitorContext context)
    : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Put("/api/election-rounds/{electionRoundId}/form-submissions/{submissionId}:setCompletion");
        DontAutoTag();
        Options(x => x.WithTags("form-submissions", "mobile"));
        Summary(s => { s.Summary = "Updates completion status status for a submission"; });

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
            .Where(x => x.MonitoringObserver.ObserverId == req.ObserverId
                        && x.MonitoringObserver.ElectionRoundId == req.ElectionRoundId
                        && x.ElectionRoundId == req.ElectionRoundId
                        && x.Id == req.SubmissionId
                        && x.Form.ElectionRoundId == req.ElectionRoundId)
            .ExecuteUpdateAsync(x => x.SetProperty(p => p.IsCompleted, req.IsCompleted), cancellationToken: ct);

        return TypedResults.NoContent();
    }
}
