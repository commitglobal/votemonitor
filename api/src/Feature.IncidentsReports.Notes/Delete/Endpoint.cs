using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Domain;

namespace Feature.IncidentsReports.Notes.Delete;

public class Endpoint(VoteMonitorContext context)
    : Endpoint<Request, Results<NoContent, NotFound, BadRequest<ProblemDetails>>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/incident-report-notes/{id}");
        DontAutoTag();
        Options(x => x.WithTags("incident-reports-notes"));
        Summary(s => { s.Summary = "Deletes a note from a incident report"; });
    }

    public override async Task<Results<NoContent, NotFound, BadRequest<ProblemDetails>>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        await context.IncidentReportNotes
            .Where(x => x.ElectionRoundId == req.ElectionRoundId && x.Id == req.Id)
            .ExecuteDeleteAsync(ct);

        return TypedResults.NoContent();
    }
}