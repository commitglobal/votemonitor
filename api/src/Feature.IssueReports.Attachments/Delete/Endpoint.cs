using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Domain;

namespace Feature.IssueReports.Attachments.Delete;

public class Endpoint(VoteMonitorContext context)
    : Endpoint<Request, Results<NoContent, NotFound, BadRequest<ProblemDetails>>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/issue-report-attachments/{id}");
        DontAutoTag();
        Options(x => x.WithTags("issue-report-attachments"));
        Summary(s => { s.Summary = "Deletes an attachment"; });
    }

    public override async Task<Results<NoContent, NotFound, BadRequest<ProblemDetails>>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        await context.IssueReportAttachments
            .Where(x => x.ElectionRoundId == req.ElectionRoundId && x.IssueReportId == req.Id)
            .ExecuteUpdateAsync(a => a.SetProperty(p => p.IsDeleted, true), ct);

        return TypedResults.NoContent();
    }
}