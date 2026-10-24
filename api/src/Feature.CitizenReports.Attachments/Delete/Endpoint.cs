using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.Entities.ReviewAggregate;

namespace Feature.CitizenReports.Attachments.Delete;

public class Endpoint(VoteMonitorContext context, IRecordAutomation recordAutomation)
    : Endpoint<Request, Results<NoContent, NotFound, BadRequest<ProblemDetails>>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/citizen-reports/{citizenReportId}/attachments/{id}");
        DontAutoTag();
        Options(x => x.WithTags("citizen-report-attachments", "public"));
        Summary(s => { s.Summary = "Deletes an attachment"; });
    }

    public override async Task<Results<NoContent, NotFound, BadRequest<ProblemDetails>>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var attachments = context.CitizenReportAttachments
            .Where(x => x.ElectionRoundId == req.ElectionRoundId
                        && x.CitizenReportId == req.CitizenReportId
                        && x.Id == req.Id
                        && !x.IsDeleted);
        // Only an uploaded attachment was part of the report.
        var wasVisible = await attachments.AnyAsync(x => x.IsCompleted, ct);
        await attachments.ExecuteUpdateAsync(a => a.SetProperty(p => p.IsDeleted, true), ct);
        if (wasVisible)
        {
            await recordAutomation.RecordChangedByAuthorAsync(req.ElectionRoundId, ReportRecordType.CitizenReport,
                req.CitizenReportId, ct);
        }

        return TypedResults.NoContent();
    }
}