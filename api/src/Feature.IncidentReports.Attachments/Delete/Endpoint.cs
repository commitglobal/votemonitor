using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Domain;

namespace Feature.IncidentReports.Attachments.Delete;

public class Endpoint(VoteMonitorContext context)
    : Endpoint<Request, Results<NoContent, NotFound, BadRequest<ProblemDetails>>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/incident-reports/{incidentReportId}/attachments/{id}");
        DontAutoTag();
        Options(x => x.WithTags("incident-report-attachments"));
        Summary(s => { s.Summary = "Deletes an attachment"; });
    }

    public override async Task<Results<NoContent, NotFound, BadRequest<ProblemDetails>>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        await context.IncidentReportAttachments
            .Where(x => x.ElectionRoundId == req.ElectionRoundId && x.IncidentReportId == req.Id)
            .ExecuteUpdateAsync(a => a.SetProperty(p => p.IsDeleted, true), ct);

        return TypedResults.NoContent();
    }
}