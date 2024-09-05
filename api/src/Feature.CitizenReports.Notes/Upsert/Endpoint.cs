using Feature.CitizenReports.Notes.Specifications;

namespace Feature.CitizenReports.Notes.Upsert;

public class Endpoint(
    IRepository<CitizenReportNoteAggregate> repository)
    : Endpoint<Request, Results<Ok<CitizenReportNoteModel>, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/citizen-report-notes");
        DontAutoTag();
        AllowAnonymous();
        Options(x => x.WithTags("citizen-reports-notes", "public"));
        Summary(s => { s.Summary = "Upserts a note for a citizen report"; });
    }

    public override async Task<Results<Ok<CitizenReportNoteModel>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var note = await repository.FirstOrDefaultAsync(new GetNoteByIdSpecification(req.ElectionRoundId, req.Id), ct);
        return note == null ? await AddNoteAsync(req, ct) : await UpdateNoteAsync(note, req, ct);
    }

    private async Task<Results<Ok<CitizenReportNoteModel>, NotFound>> UpdateNoteAsync(CitizenReportNoteAggregate note,
        Request req, CancellationToken ct)
    {
        note.UpdateText(req.Text);
        await repository.UpdateAsync(note, ct);

        return TypedResults.Ok(CitizenReportNoteModel.FromEntity(note));
    }

    private async Task<Results<Ok<CitizenReportNoteModel>, NotFound>> AddNoteAsync(Request req, CancellationToken ct)
    {
        var note = new CitizenReportNoteAggregate(req.Id,
            req.ElectionRoundId,
            req.CitizenReportId,
            req.FormId,
            req.QuestionId,
            req.Text);

        await repository.AddAsync(note, ct);

        return TypedResults.Ok(CitizenReportNoteModel.FromEntity(note));
    }
}