using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Domain;

namespace Feature.CitizenReports.Notes.Delete;

public class Endpoint(VoteMonitorContext context)
    : Endpoint<Request, Results<NoContent, NotFound, BadRequest<ProblemDetails>>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/citizen-report-notes/{id}");
        DontAutoTag();
        AllowAnonymous();
        Options(x => x.WithTags("citizen-reports-notes", "public"));
        Summary(s => { s.Summary = "Deletes a note from a citizen report"; });
    }

    public override async Task<Results<NoContent, NotFound, BadRequest<ProblemDetails>>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        await context.CitizenReportNotes
            .Where(x => x.ElectionRoundId == req.ElectionRoundId && x.Id == req.Id)
            .ExecuteDeleteAsync(ct);

        return TypedResults.NoContent();
    }
}