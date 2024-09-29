using Feature.IssueReports.Notes.Specifications;

namespace Feature.IssueReports.Notes.Upsert;

public class Endpoint(
    IRepository<IssueReportNoteAggregate> repository)
    : Endpoint<Request, Results<Ok<IssueReportNoteModel>, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/issue-report-notes");
        DontAutoTag();
        Options(x => x.WithTags("issue-reports-notes"));
        Summary(s => { s.Summary = "Upserts a note for a issue report"; });
        
    }

    public override async Task<Results<Ok<IssueReportNoteModel>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var note = await repository.FirstOrDefaultAsync(new GetNoteByIdSpecification(req.ElectionRoundId, req.Id), ct);
        return note == null ? await AddNoteAsync(req, ct) : await UpdateNoteAsync(note, req, ct);
    }

    private async Task<Results<Ok<IssueReportNoteModel>, NotFound>> UpdateNoteAsync(IssueReportNoteAggregate note,
        Request req, CancellationToken ct)
    {
        note.UpdateText(req.Text);
        await repository.UpdateAsync(note, ct);

        return TypedResults.Ok(IssueReportNoteModel.FromEntity(note));
    }

    private async Task<Results<Ok<IssueReportNoteModel>, NotFound>> AddNoteAsync(Request req, CancellationToken ct)
    {
        var note = new IssueReportNoteAggregate(req.Id,
            req.ElectionRoundId,
            req.IssueReportId,
            req.FormId,
            req.QuestionId,
            req.Text);

        await repository.AddAsync(note, ct);

        return TypedResults.Ok(IssueReportNoteModel.FromEntity(note));
    }
}