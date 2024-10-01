using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Domain;

namespace Feature.CitizenReports.Attachments.Delete;

public class Endpoint(VoteMonitorContext context)
    : Endpoint<Request, Results<NoContent, NotFound, BadRequest<ProblemDetails>>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/citizen-report-attachments/{id}");
        DontAutoTag();
        Options(x => x.WithTags("citizen-report-attachments", "public"));
        Summary(s => { s.Summary = "Deletes an attachment"; });
    }

    public override async Task<Results<NoContent, NotFound, BadRequest<ProblemDetails>>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        await context.CitizenReportAttachments
            .Where(x => x.ElectionRoundId == req.ElectionRoundId && x.CitizenReportId == req.Id)
            .ExecuteUpdateAsync(a => a.SetProperty(p => p.IsDeleted, true), ct);

        return TypedResults.NoContent();
    }
}