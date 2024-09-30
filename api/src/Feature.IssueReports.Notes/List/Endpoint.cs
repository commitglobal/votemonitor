using Feature.IssueReports.Notes.Specifications;

namespace Feature.IssueReports.Notes.List;

public class Endpoint(
    IReadRepository<IssueReportNoteAggregate> repository)
    : Endpoint<Request, Response>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/issue-report-notes");
        DontAutoTag();
        Options(x => x.WithTags("issue-reports-notes"));
        Summary(s => { s.Summary = "Lists notes for a issue report"; });
    }

    public override async Task<Response> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var specification = new ListNotesSpecification(req.ElectionRoundId, req.IssueReportId);
        var notes = await repository.ListAsync(specification, ct);

        return new Response { Notes = notes };
    }
}