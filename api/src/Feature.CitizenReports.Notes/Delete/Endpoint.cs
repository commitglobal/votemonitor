using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.Entities.ReviewAggregate;

namespace Feature.CitizenReports.Notes.Delete;

public class Endpoint(VoteMonitorContext context, IRecordAutomation recordAutomation)
    : Endpoint<Request, Results<NoContent, NotFound, BadRequest<ProblemDetails>>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/citizen-reports/{citizenReportId}/notes/{id}");
        DontAutoTag();
        AllowAnonymous();
        Options(x => x.WithTags("citizen-report-notes", "public"));
        Summary(s => { s.Summary = "Deletes a note from a citizen report"; });
    }

    public override async Task<Results<NoContent, NotFound, BadRequest<ProblemDetails>>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var deleted = await context.CitizenReportNotes
            .Where(x => x.ElectionRoundId == req.ElectionRoundId && x.CitizenReportId == req.CitizenReportId && x.Id == req.Id)
            .ExecuteDeleteAsync(ct);
        if (deleted > 0)
        {
            await recordAutomation.RecordChangedByAuthorAsync(req.ElectionRoundId, ReportRecordType.CitizenReport,
                req.CitizenReportId, ct);
        }

        return TypedResults.NoContent();
    }
}