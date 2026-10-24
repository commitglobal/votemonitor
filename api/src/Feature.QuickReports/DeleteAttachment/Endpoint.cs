using Authorization.Policies.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.Entities.ReviewAggregate;

namespace Feature.QuickReports.DeleteAttachment;

public class Endpoint(
    IAuthorizationService authorizationService,
    VoteMonitorContext context,
    IRecordAutomation recordAutomation)
    : Endpoint<Request, Results<NoContent, NotFound, BadRequest<ProblemDetails>>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/quick-reports/{quickReportId}/attachments/{id}");
        DontAutoTag();
        Options(x => x.WithTags("quick-reports", "mobile"));
        Summary(s =>
        {
            s.Summary = "Deletes a quick report and it's attachments";
        });
    }

    public override async Task<Results<NoContent, NotFound, BadRequest<ProblemDetails>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult = await authorizationService.AuthorizeAsync(User, new MonitoringObserverRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        // Mark as deleted so our background job will delete it. also change the id in order to allow recreation with same id
        var attachments = context
            .QuickReportAttachments
            .Where(x => x.ElectionRoundId == req.ElectionRoundId
                        && x.MonitoringObserver.ObserverId == req.ObserverId
                        && x.MonitoringObserver.ElectionRoundId == req.ElectionRoundId
                        && x.MonitoringObserver.MonitoringNgo.ElectionRoundId == req.ElectionRoundId
                        && x.QuickReportId == req.QuickReportId
                        && x.Id == req.Id);
        // Only an uploaded attachment was part of the report.
        var wasVisible = await attachments.AnyAsync(x => x.IsCompleted && !x.IsDeleted, ct);
        await attachments.ExecuteUpdateAsync(setters => setters
                .SetProperty(qra => qra.IsDeleted, true)
                .SetProperty(qra => qra.Id, Guid.NewGuid()), cancellationToken: ct);
        if (wasVisible)
        {
            await recordAutomation.RecordChangedByAuthorAsync(req.ElectionRoundId, ReportRecordType.QuickReport,
                req.QuickReportId, ct);
        }

        return TypedResults.NoContent();
    }
}
