using Feature.CitizenReports.Notes.Specifications;

namespace Feature.CitizenReports.Notes.List;

public class Endpoint(
    IReadRepository<CitizenReportNoteAggregate> repository)
    : Endpoint<Request, Response>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/citizen-reports/{citizenReportId}/notes");
        DontAutoTag();
        AllowAnonymous();
        Options(x => x.WithTags("citizen-report-notes", "public"));
        Summary(s => { s.Summary = "Lists notes for a citizen report"; });
    }

    public override async Task<Response> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var specification = new ListNotesSpecification(req.ElectionRoundId, req.CitizenReportId);
        var notes = await repository.ListAsync(specification, ct);

        return new Response { Notes = notes };
    }
}