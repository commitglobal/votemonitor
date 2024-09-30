using Feature.IncidentsReports.Notes.Specifications;

namespace Feature.IncidentsReports.Notes.Get;

public class Endpoint(IReadRepository<IncidentReportNoteAggregate> repository)
    : Endpoint<Request, Results<Ok<IncidentReportNoteModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/incident-report-notes/{id}");
        DontAutoTag();
        Options(x => x.WithTags("incident-reports-notes"));
        Summary(s => { s.Summary = "Gets a note by id for a incident report"; });
    }

    public override async Task<Results<Ok<IncidentReportNoteModel>, NotFound>> ExecuteAsync(
        Request req, CancellationToken ct)
    {
        var note = await repository.FirstOrDefaultAsync(new GetNoteByIdSpecification(req.ElectionRoundId, req.Id), ct);
        if (note is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(IncidentReportNoteModel.FromEntity(note));
    }
}