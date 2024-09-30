using Feature.IssueReports.Notes.Specifications;

namespace Feature.IssueReports.Notes.Get;

public class Endpoint(IReadRepository<IssueReportNoteAggregate> repository)
    : Endpoint<Request, Results<Ok<IssueReportNoteModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/issue-report-notes/{id}");
        DontAutoTag();
        Options(x => x.WithTags("issue-reports-notes"));
        Summary(s => { s.Summary = "Gets a note by id for a issue report"; });
    }

    public override async Task<Results<Ok<IssueReportNoteModel>, NotFound>> ExecuteAsync(
        Request req, CancellationToken ct)
    {
        var note = await repository.FirstOrDefaultAsync(new GetNoteByIdSpecification(req.ElectionRoundId, req.Id), ct);
        if (note is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(IssueReportNoteModel.FromEntity(note));
    }
}