using Feature.IncidentReports.Notes.Specifications;

namespace Feature.IncidentReports.Notes.List;

public class Endpoint(
    IReadRepository<IncidentReportNoteAggregate> repository)
    : Endpoint<Request, Response>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/incident-reports/{incidentReportId}/notes");
        DontAutoTag();
        Options(x => x.WithTags("incident-report-notes"));
        Summary(s => { s.Summary = "Lists notes for a incident report"; });
    }

    public override async Task<Response> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var specification = new ListNotesSpecification(req.ElectionRoundId, req.IncidentReportId);
        var notes = await repository.ListAsync(specification, ct);

        return new Response { Notes = notes };
    }
}