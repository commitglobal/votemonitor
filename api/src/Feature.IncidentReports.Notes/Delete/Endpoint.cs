using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Domain;
using Feature.IncidentReports.Notes.Specifications;
using Vote.Monitor.Domain.Entities.IncidentReportAggregate;
using Vote.Monitor.Domain.Entities.ReviewAggregate;

namespace Feature.IncidentReports.Notes.Delete;

public class Endpoint(
    VoteMonitorContext context,
    IReadRepository<IncidentReport> incidentReportRepository,
    IRecordAutomation recordAutomation)
    : Endpoint<Request, Results<NoContent, NotFound, Conflict, BadRequest<ProblemDetails>>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/incident-reports/{incidentReportId}/notes/{id}");
        DontAutoTag();
        Options(x => x.WithTags("incident-report-notes"));
        Summary(s => { s.Summary = "Deletes a note from a incident report"; });
    }

    public override async Task<Results<NoContent, NotFound, Conflict, BadRequest<ProblemDetails>>> ExecuteAsync(
        Request req,
        CancellationToken ct)
    {
        // a completed incident report is locked
        if (await incidentReportRepository.AnyAsync(
                new CompletedIncidentReportSpecification(req.ElectionRoundId, req.IncidentReportId), ct))
        {
            return TypedResults.Conflict();
        }

        var deleted = await context.IncidentReportNotes
            .Where(x => x.ElectionRoundId == req.ElectionRoundId
                        && x.IncidentReportId == req.IncidentReportId
                        && x.Id == req.Id)
            .ExecuteDeleteAsync(ct);
        if (deleted > 0)
        {
            await recordAutomation.RecordChangedByAuthorAsync(req.ElectionRoundId, ReportRecordType.IncidentReport,
                req.IncidentReportId, ct);
        }

        return TypedResults.NoContent();
    }
}