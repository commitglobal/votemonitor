using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Domain;

namespace Feature.IssueReports.Notes.Delete;

public class Endpoint(VoteMonitorContext context)
    : Endpoint<Request, Results<NoContent, NotFound, BadRequest<ProblemDetails>>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/issue-report-notes/{id}");
        DontAutoTag();
        Options(x => x.WithTags("issue-reports-notes"));
        Summary(s => { s.Summary = "Deletes a note from a issue report"; });
    }

    public override async Task<Results<NoContent, NotFound, BadRequest<ProblemDetails>>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        await context.IssueReportNotes
            .Where(x => x.ElectionRoundId == req.ElectionRoundId && x.Id == req.Id)
            .ExecuteDeleteAsync(ct);

        return TypedResults.NoContent();
    }
}