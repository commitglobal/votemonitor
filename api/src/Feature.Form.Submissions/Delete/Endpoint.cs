using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.Entities.PollingStationCorrectionAggregate;
using Vote.Monitor.Domain.Entities.RecordSeverityAggregate;
using Vote.Monitor.Domain.Entities.ReviewAggregate;

namespace Feature.Form.Submissions.Delete;

public class Endpoint(IAuthorizationService authorizationService, VoteMonitorContext context)
    : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/form-submissions/{submissionId}");
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

        var deleted = await context.FormSubmissions
            .Where(x => x.ElectionRoundId == req.ElectionRoundId
                        && x.Id == req.SubmissionId
                        && x.Form.ElectionRoundId == req.ElectionRoundId
                        && x.MonitoringObserver.ObserverId == req.ObserverId
                        && x.MonitoringObserver.ElectionRoundId == req.ElectionRoundId)
            .ExecuteDeleteAsync(ct);

        if (deleted > 0)
        {
            await context.PollingStationCorrections
                .Where(x => x.ElectionRoundId == req.ElectionRoundId
                            && x.RecordType == CorrectableRecordType.FormSubmission
                            && x.RecordId == req.SubmissionId)
                .ExecuteDeleteAsync(ct);

            await context.DeleteReviewAsync(ReportRecordType.FormSubmission, req.SubmissionId, ct);
            await context.DeleteSeverityAsync(ReportRecordType.FormSubmission, req.SubmissionId, ct);
        }

        await context.Notes
            .Where(x => x.SubmissionId == req.SubmissionId
                        && x.MonitoringObserver.ObserverId == req.ObserverId
                        && x.MonitoringObserver.ElectionRoundId == req.ElectionRoundId)
            .ExecuteDeleteAsync(ct);

        await context.Attachments
            .Where(x => x.MonitoringObserver.ObserverId == req.ObserverId
                        && x.MonitoringObserver.ElectionRoundId == req.ElectionRoundId
                        && x.SubmissionId == req.SubmissionId)
            .ExecuteUpdateAsync(x => x.SetProperty(a => a.IsDeleted, true), ct);

        return TypedResults.NoContent();
    }
}
