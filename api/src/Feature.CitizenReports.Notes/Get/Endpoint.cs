using Feature.CitizenReports.Notes.Specifications;

namespace Feature.CitizenReports.Notes.Get;

public class Endpoint(IReadRepository<CitizenReportNoteAggregate> repository)
    : Endpoint<Request, Results<Ok<CitizenReportNoteModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/citizen-report-notes/{id}");
        DontAutoTag();
        AllowAnonymous();
        Options(x => x.WithTags("citizen-reports-notes", "public"));
        Summary(s => { s.Summary = "Gets a note by id for a citizen report"; });
    }

    public override async Task<Results<Ok<CitizenReportNoteModel>, NotFound>> ExecuteAsync(
        Request req, CancellationToken ct)
    {
        var note = await repository.FirstOrDefaultAsync(new GetNoteByIdSpecification(req.ElectionRoundId, req.Id), ct);
        if (note is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(CitizenReportNoteModel.FromEntity(note));
    }
}