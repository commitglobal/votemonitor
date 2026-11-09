using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Domain;
using Feature.IncidentReports.Attachments.Specifications;
using Vote.Monitor.Domain.Entities.IncidentReportAggregate;
using Vote.Monitor.Domain.Entities.ReviewAggregate;

namespace Feature.IncidentReports.Attachments.Delete;

public class Endpoint(
    VoteMonitorContext context,
    IReadRepository<IncidentReport> incidentReportRepository,
    IRecordAutomation recordAutomation)
    : Endpoint<Request, Results<NoContent, NotFound, Conflict, BadRequest<ProblemDetails>>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/incident-reports/{incidentReportId}/attachments/{id}");
        DontAutoTag();
        Options(x => x.WithTags("incident-report-attachments"));
        Summary(s => { s.Summary = "Deletes an attachment"; });
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

        var attachments = context.IncidentReportAttachments
            .Where(x => x.ElectionRoundId == req.ElectionRoundId
                        && x.IncidentReportId == req.IncidentReportId
                        && x.Id == req.Id
                        && !x.IsDeleted);
        // Only an uploaded attachment was part of the report.
        var wasVisible = await attachments.AnyAsync(x => x.IsCompleted, ct);
        await attachments.ExecuteUpdateAsync(a => a.SetProperty(p => p.IsDeleted, true), ct);
        if (wasVisible)
        {
            await recordAutomation.RecordChangedByAuthorAsync(req.ElectionRoundId, ReportRecordType.IncidentReport,
                req.IncidentReportId, ct);
        }

        return TypedResults.NoContent();
    }
}