using Feature.IncidentsReports.Notes.Specifications;

namespace Feature.IncidentsReports.Notes.Upsert;

public class Endpoint(
    IRepository<IncidentReportNoteAggregate> repository)
    : Endpoint<Request, Results<Ok<IncidentReportNoteModel>, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/incident-report-notes");
        DontAutoTag();
        Options(x => x.WithTags("incident-reports-notes"));
        Summary(s => { s.Summary = "Upserts a note for a incident report"; });
        
    }

    public override async Task<Results<Ok<IncidentReportNoteModel>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var note = await repository.FirstOrDefaultAsync(new GetNoteByIdSpecification(req.ElectionRoundId, req.Id), ct);
        return note == null ? await AddNoteAsync(req, ct) : await UpdateNoteAsync(note, req, ct);
    }

    private async Task<Results<Ok<IncidentReportNoteModel>, NotFound>> UpdateNoteAsync(IncidentReportNoteAggregate note,
        Request req, CancellationToken ct)
    {
        note.UpdateText(req.Text);
        await repository.UpdateAsync(note, ct);

        return TypedResults.Ok(IncidentReportNoteModel.FromEntity(note));
    }

    private async Task<Results<Ok<IncidentReportNoteModel>, NotFound>> AddNoteAsync(Request req, CancellationToken ct)
    {
        var note = new IncidentReportNoteAggregate(req.Id,
            req.ElectionRoundId,
            req.IncidentReportId,
            req.FormId,
            req.QuestionId,
            req.Text);

        await repository.AddAsync(note, ct);

        return TypedResults.Ok(IncidentReportNoteModel.FromEntity(note));
    }
}